# Execute only if running in container:
if (nzchar(Sys.getenv("AML_APP_ROOT"))) {
  entry_script_path <- paste0(
    Sys.getenv("AML_APP_ROOT"),
    "/",
    Sys.getenv("AZUREML_ENTRY_SCRIPT")
  )

  runner <- plumber::plumb(entry_script_path)

  args <- list(host = "0.0.0.0", port = 8000)
  do.call(runner$run, args)
}
