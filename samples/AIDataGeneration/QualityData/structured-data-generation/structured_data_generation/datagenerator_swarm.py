from __future__ import annotations
import json
import time
import types
from typing import Callable, Dict, List, Type, Union
from swarm import Agent
from pydantic import BaseModel
from swarm import Agent, Swarm
from openai import AzureOpenAI
from openai.types.completion_usage import CompletionUsage
from swarm.types import AgentFunction

class GeneratedElements(BaseModel):
    elements: Dict[Type[BaseModel], List[BaseModel]] = {}
    usage: CompletionUsage = CompletionUsage(completion_tokens=0, prompt_tokens=0, total_tokens=0)
    total_time: int = 0

    def list_as_json(self, type: Type[BaseModel]):
        return [e.model_dump_json(exclude=None) for e in self.elements[type]]

    def add_usage(self, usage: CompletionUsage):
        self.usage.completion_tokens += usage.completion_tokens
        self.usage.prompt_tokens += usage.prompt_tokens
        self.usage.total_tokens += usage.total_tokens

    def model_dump_json(self, exclude_none: bool = True, indent: int = 2) -> str:
        result = {}
        for model_type, elements in self.elements.items():
            result[model_type.__name__] = [e.model_dump(exclude_none=exclude_none) for e in elements]
        result["usage"] = self.usage.model_dump(exclude_none=exclude_none)
        result["usage"]["total_time"] = self.total_time
        
        return json.dumps(result, indent=indent)

class ElementCreatorAgent(Agent):
    element_type: Type[BaseModel] = None
    tool_choice: str = "auto"

    def __init__(self) -> None:
        super().__init__()
        if not self.instructions:
            self.instructions = self._instructions
        if not self.name or self.name == "Agent":
            self.name = f"{self.element_type.__name__} Creator Agent"
        if not self.functions:
            self.functions = [self._get_store_element_function()]

    def _instructions(self) -> str:
        return (
            f"You are a synthetic data creator agent that generates data of type {self.element_type.__name__}. "
            f"Make sure to provide/make up data for all the required fields in the {self.element_type.__name__}. "
            f"After creating data of type {self.element_type.__name__}, you store it using the store_{self.element_type.__name__.lower()} function."
        )

    def _get_elements_of_type(context_variables: dict, element_type: Type[BaseModel]) -> List[BaseModel]:
        generated_elements: GeneratedElements = context_variables["generated_elements"]
        if element_type not in generated_elements.elements:
            return []

        elements = generated_elements.elements[element_type]
        return elements

    def _store_element(self, context_variables: dict, model_element: BaseModel) -> bool:
        generated_elements: GeneratedElements = context_variables["generated_elements"]
        pretty_print("  Storing", f"{model_element.__class__.__name__} in generated elements in {self.__class__.__name__}.")
        if self.element_type not in generated_elements.elements:
            generated_elements.elements[self.element_type] = []
        
        generated_elements.elements[self.element_type].append(model_element)
        
        return True

    def _get_store_element_function(self) -> types.FunctionType:
        def _store_element_func(context_variables, element) -> bool:
            return self._store_element(context_variables, element)

        func_wrapper = types.FunctionType(
            _store_element_func.__code__,
            _store_element_func.__globals__,
            f"store_{self.element_type.__name__.lower()}",
            argdefs=None,
            closure=_store_element_func.__closure__)
        
        func_wrapper.__doc__ = f"""
            Stores a {self.element_type.__name__} element in the generated elements.",
            This function must be called after generating an element of type {self.element_type.__name__}.
        """
        func_wrapper.__annotations__ = {'context_variables': 'dict', 'element': self.element_type.__qualname__, 'return': 'bool'}
        func_wrapper.__globals__[self.element_type.__qualname__] = self.element_type

        return func_wrapper

    def get_agent_function(self) -> types.FunctionType:
        func = lambda: self

        func_wrapper = types.FunctionType(
            func.__code__,
            func.__globals__,
            f"create_{self.element_type.__name__.lower()}",
            argdefs=None,
            closure=func.__closure__)
        
        func_wrapper.__doc__ = self.element_type.__doc__

        return func_wrapper

def pretty_print(key: str, message: str) -> None:
    print(f"\033[92m{key}\033[0m: {message}")

def pretty_print_messages(messages) -> None:
    for message in messages:
        if message["role"] != "assistant":
            continue

        # print agent name in blue
        print(f"\033[94m{message['sender']}\033[0m:", end=" ")

        # print response, if any
        if message["content"]:
            print(message["content"])

        # print tool calls in purple, if any
        tool_calls = message.get("tool_calls") or []
        if len(tool_calls) > 1:
            print()
        for tool_call in tool_calls:
            f = tool_call["function"]
            name, args = f["name"], f["arguments"]
            arg_str = json.dumps(json.loads(args)).replace(":", "=")
            print(f"\033[95m{name}\033[0m({arg_str[1:-1]})")

class Tasks(BaseModel):
    tasks: List[Task] = None

class Task(BaseModel):
    """A task that can be executed by the creator agent"""
    name: str
    description: str
    creator_agent: str

class PlannerAgent(Agent):
    name: str = "Planner"

    def add_tasks(context_variables: dict, tasks: Tasks):
        """Add tasks that are later to be executed"""
        pretty_print("Adding tasks", f"{tasks.model_dump_json(indent=2, exclude_none=True)}")
        context_variables["tasks"].extend(tasks.tasks)

    functions: List[AgentFunction] = [add_tasks]
    tool_choice: str = "auto"

    def instructions(context_variables: dict) -> str :
        creators: List[ElementCreatorAgent] = context_variables["creators"]
        return f"""
        You are a planner agent that plans the generation of synthetic data.
        You create a plan for tasks that needs to be done to create the data.

        These are the creator agent that the orchestrator can subsequently use for the tasks:

        {[f"{creator.name}: {creator.__doc__}" for creator in creators]}.
        """

class OrchestratorAgent(Agent):
    name: str = "Orchestrator"

    instructions: Union[str, Callable[[], str]] = """
        You are an orchestrator agent executes synthetic data generators according
        to an input task.
        """
    tool_choice: str = "auto"
    
    def __init__(self, creators: List[ElementCreatorAgent]):
        super().__init__()
        self.functions = [creator.get_agent_function() for creator in creators]

class DataGeneratorSwarm(Swarm):
    def __init__(
            self,
            client: AzureOpenAI=None,
            creators: List[ElementCreatorAgent]=[],
            debug=False):
        super().__init__(client)
        self._planner = PlannerAgent()
        self._orchestrator = OrchestratorAgent(creators)
        self._context_variables = {"creators": creators, "tasks": []}
        self._debug = debug

    def run(self, request: str) -> GeneratedElements:
        messages = [{"role": "user", "content": request}]

        start_time = time.time()

        result = super().run(
            self._planner,
            messages,
            context_variables=self._context_variables)

        if self._debug:
            pretty_print_messages(result.messages)

        self. generated_elements = GeneratedElements()
        self.generated_elements.add_usage(result.usage)

        for task in result.context_variables["tasks"]:
            pretty_print("Executing task", task.model_dump_json(indent=2, exclude_none=True))
            result = super().run(
                self._orchestrator,
                [{"role": "user", "content": task.model_dump_json()}],
                context_variables={"generated_elements": self.generated_elements}
            )
            self.generated_elements: GeneratedElements = result.context_variables["generated_elements"]
            self.generated_elements.add_usage(result.usage)
            if self._debug:
                pretty_print_messages(result.messages)

        end_time = time.time()
        self.generated_elements.total_time = end_time - start_time

        return self.generated_elements
