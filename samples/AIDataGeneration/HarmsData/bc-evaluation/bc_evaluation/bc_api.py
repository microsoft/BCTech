# ---------------------------------------------------------
# Copyright (c) Microsoft Corporation. All rights reserved.
# ---------------------------------------------------------

from flask import Flask, jsonify, request

from bc_evaluation import bc_simulation
from bc_evaluation import bc_utils

azure_ai_project = bc_utils.get_azure_ai_project()
simulations = {}
app = Flask(__name__)

@app.route('/simulation', methods=['POST'])
def post_simulation():
    data = request.get_json()

    scenario = data["scenario"]
    max_simulation_results = data["max_simulation_results"]
    max_conversation_turns = data["max_conversation_turns"]
    randomization_seed = data["randomization_seed"]
    upia = data["upia"]

    id = str(len(simulations))
    simulation = bc_simulation.Simulation(
        id,
        scenario,
        max_simulation_results,
        max_conversation_turns,
        randomization_seed,
        upia,
        azure_ai_project)
    
    simulations[simulation.id] = simulation

    print(f"Simulation {simulation.id} created with scenario {scenario}, max_simulation_results {max_simulation_results}, max_conversation_turns {max_conversation_turns}")

    simulation.start()

    return jsonify({ 'id': simulation.id })

@app.route('/simulation/<simulation_id>/queries', methods=['GET'])
def get_simulation_query(simulation_id):
    try:
        simulation = simulations[simulation_id]
    except KeyError:
        return jsonify({}), 404

    message = simulation.get_query()

    return jsonify(message)

@app.route('/simulation/<simulation_id>/queries/peek', methods=['GET'])
def peek_simulation_query(simulation_id):
    try:
        simulation = simulations[simulation_id]
    except KeyError:
        return jsonify({}), 404

    message = simulation.peek_query()
    
    return jsonify(message)

@app.route('/simulation/<simulation_id>/responses', methods=['PUT'])
def put_simulation_response(simulation_id):
    try:
        simulation = simulations[simulation_id]
    except KeyError:
        return jsonify({}), 404
    
    message = request.get_json()["response"]

    simulation.put_response(message)

    return jsonify({}), 200

@app.route('/simulation/<simulation_id>', methods=['GET'])
def get_simulation(simulation_id):
    try:
        simulation = simulations[simulation_id]
    except KeyError:
        return jsonify({}), 404

    return jsonify(simulation.to_dict())

@app.route('/simulation/<simulation_id>', methods=['DELETE'])
def delete_simulation(simulation_id):
    try:
        simulation = simulations[simulation_id]
    except KeyError:
        return jsonify({}), 404

    simulation.stop()
    del simulations[simulation_id]

    return jsonify({}), 200

def start_server(port=8000):
    app.run(debug=True, port=port)
