import click
from bc_evaluation import bc_api

@click.group(help="CLI for BC AI Evaluation")
def cli():
    pass

@cli.group()
def api():
    pass

@api.command("start", help="Run BC AI Evaluation API")
@click.option("--port", default=8000, help="Port to run the API on")
def start_evaluation_api(port: int):
    """Start the BC AI Evaluation server."""
    bc_api.start_server(port)

if __name__ == "__main__":
    cli()
