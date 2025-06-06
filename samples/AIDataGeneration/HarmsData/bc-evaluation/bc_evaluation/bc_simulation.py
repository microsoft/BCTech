# ---------------------------------------------------------
# Copyright (c) Microsoft Corporation. All rights reserved.
# ---------------------------------------------------------

import math
import asyncio
import queue
import threading

import azure.ai.evaluation.simulator as aais_simulator
from azure.identity import DefaultAzureCredential

ADVERSARIAL_INDIRECT_JAILBREAK = "ADVERSARIAL_INDIRECT_JAILBREAK"


class PeekableQueue(queue.Queue):
    def peek(self):
        with self.not_empty:
            while not self._qsize():
                self.not_empty.wait()
            return self.queue[0]
        
class ResponseQueue(queue.Queue):
    def clear_waiters(self):
        # Make sure that all waiting threads are released
        print(f"\nClearing {len(self.not_empty._waiters)} waiter(s).\n")
        [self.put("continue") for _ in range(len(self.not_empty._waiters)) ]

class Simulation():
    def __init__(
            self,
            id,
            scenario,
            max_simulation_results,
            max_conversation_turns,
            randomization_seed,
            upia,
            azure_ai_project):

        self.id = id
        self.scenario = scenario
        self.max_simulation_results = max_simulation_results
        self.simulation_result_number = 0
        self.max_conversation_turns = max_conversation_turns
        self.conversation_turn_number = 0
        self.randomization_seed = randomization_seed
        self.queries = PeekableQueue()
        self.responses = ResponseQueue()

        simulator_params = {
            "scenario": aais_simulator.AdversarialScenario.__members__.get(scenario),
            "target": self._callback,
            "max_simulation_results": max_simulation_results,
            "max_conversation_turns": max_conversation_turns,
            "concurrent_async_task": 1,
            "randomization_seed": randomization_seed,
        }
        if upia:
            simulator_params["_jailbreak_type"] = "upia"

        print(f"SCENARIO: {scenario}")
        simulator_class = aais_simulator.AdversarialSimulator
        if (scenario == ADVERSARIAL_INDIRECT_JAILBREAK):
            simulator_class = aais_simulator.IndirectAttackSimulator
            del simulator_params["randomization_seed"] # Seed is not supported for XPIA
            del simulator_params["scenario"] # Scenario is not supported for XPIA

        simulator = simulator_class(azure_ai_project=azure_ai_project, credential=DefaultAzureCredential())

        print(f"\nSimulator {simulator} created with parameters {simulator_params}\n")

        simulation = simulator(**simulator_params)

        print(f"\nSimulation {self.id} created with parameters {simulator_params}\n")

        # Create a separate thread for the simulation to not block the API
        self.thread = threading.Thread(target=asyncio.run, args=(simulation,))

    async def _callback(self, messages, stream: bool = False, session_state = None, context = None):
        self.conversation_turn_number = math.ceil(len(messages["messages"]) / 2)
        if (self.conversation_turn_number == 1):
            self.simulation_result_number += 1

        query = {
            "query": messages["messages"][-1]["content"],
            "messages": messages,
            "conversation_turn_number": self.conversation_turn_number,
            "simulation_result_number": self.simulation_result_number,
        }

        template_parameters = messages["template_parameters"]

        if "xpia_attack_sentence" in template_parameters:
            # For XPIA, the harm without injection is in the templates
            query["xpia_attack_sentence"] = template_parameters["xpia_attack_sentence"]
            query["query"] = template_parameters["content"]
        
        if "file_content" in template_parameters and "filename" in template_parameters:
            query["query"] += f"\n\n{template_parameters['filename']}:\n\n{template_parameters['file_content']}"

        self.queries.put(query)

        print(f"\nQuery:\n   - '{query['query'][0:100]}'\n   - result: {self.simulation_result_number}\n   - turn: {self.conversation_turn_number}\n")

        response = self.responses.get()
        print(f"\nResponse:\n   - '{response}'\n   - result: {self.simulation_result_number}\n   - turn: {self.conversation_turn_number}\n")

        messages["messages"].append({
            "content": response,
            "role": "assistant",
            "context": None,
        })

        return {
            "messages": messages["messages"],
            "stream": stream,
            "session_state": session_state,
            "context": context
        }

    def start(self):
        self.thread.start()

    def stop(self):
        pass
        
    def put_response(self, message):
        self.responses.put(message)
    
    def peek_query(self):
        self._ensure_no_block()
        return self.queries.peek()

    def get_query(self):
        self._ensure_no_block()
        return self.queries.get()
    
    def _ensure_no_block(self):
        if self.queries.qsize() == 0:
            # Query was consumed, but we never responded
            # need to clear waitiers to not block
            self.responses.clear_waiters()

    def to_dict(self):
        return {
            "id": self.id,
            "scenario": self.scenario,
            "max_simulation_results": self.max_simulation_results,
            "max_conversation_turns": self.max_conversation_turns,
            "randomization_seed": self.randomization_seed
        }
