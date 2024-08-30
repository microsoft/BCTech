# Execute only if running in container:
if (nzchar(Sys.getenv("AML_APP_ROOT"))) {
  source("./forecast.R")
  source("./prediction.R")
}

#* Liveness check
#* @get /live
function() {
  "alive"
}

#* Readiness check
#* @get /ready
function() {
  "ready"
}

#* @serializer json list(dataframe="values")
#* @param Inputs
#* @param GlobalParameters
#* @post /score
function(Inputs, GlobalParameters, res) { # nolint: object_name_linter.
  return(execute_score(Inputs, GlobalParameters, res))
}

#* @serializer json list(dataframe="values")
#* @param Inputs
#* @param GlobalParameters
#* @post /score/execute
function(Inputs, GlobalParameters, res) { # nolint: object_name_linter.
  return(execute_score(Inputs, GlobalParameters, res))
}

execute_score <- function(Inputs, GlobalParameters, res) {
  tryCatch({
    start_time <- Sys.time()

    parameters <- read.csv(
      text = GlobalParameters$Parameters,
      header = TRUE,
      stringsAsFactors = FALSE)
    is_forecast_request <- grepl(
      "TIME_SERIES_MODEL",
      GlobalParameters$Parameters,
      ignore.case = TRUE)

    if (is_forecast_request) {
      output <- execute_forecast(Inputs, parameters)
    } else {
      output <- execute_predict(Inputs, parameters)
    }

    # BC expects output on the form:
    # { "Results": { "Output1" : { "value" : { "Values": [ [ ... ], ... ]} } } }
    result <- list(
      Results = list(
        Output1 = list(
          value = list(
            Values = output
          )
        )
      )
    )

    end_time <- Sys.time()
    res$setHeader(
      "x-ms-request-duration",
      format_timespan(start_time, end_time))

    return(result)
  }, error = function(e) {
    print(paste("Error:", e))
    res$status <- 500
    return(list(error=paste0(e)))
  })
}

execute_forecast <- function(inputs, parameters) {
  cat("Forecast using time series model: ", parameters$TIME_SERIES_MODEL, "\n")

  data <- convert_inputs_to_dataframe(inputs)

  output <- smart.forecast(data, parameters)

  # BC expects filtering of rows (to include those with "Type" == 1)
  # and dropping the "Type" column
  output <- subset(output, Type == 1)
  output <- subset(output, select = -Type)

  return(output)
}

execute_predict <- function(inputs, parameters) {
  cat("Predict using method: ", parameters$method, "\n")

  data <- convert_inputs_to_dataframe(inputs)
  data[data == ""] <- NA

  output <- smart.prediction(data, parameters)

  return(output)
}

convert_inputs_to_dataframe <- function(inputs) {
  df <- as.data.frame(inputs$input1$Values)
  colnames(df) <- gsub(" ", "_", inputs$input1$ColumnNames)

  return(df)
}

format_timespan <- function(start_time, end_time) {
  # Format the difference between start_time and end_time to be
  # able to be parsed by .NET TimeSpan.Parse()
  total_seconds <- as.numeric(difftime(end_time, start_time, units = "secs"))

  days <- floor(total_seconds / (24 * 3600))
  hours <- floor((total_seconds %% (24 * 3600)) / 3600)
  minutes <- floor((total_seconds %% 3600) / 60)
  seconds <- floor(total_seconds %% 60)
  milliseconds <- floor(10000000 * (total_seconds - trunc(total_seconds)))

  formatted_timespan <- sprintf(
                                "%d.%02d:%02d:%02d.%07d",
                                days,
                                hours,
                                minutes,
                                seconds,
                                milliseconds)

  return(formatted_timespan)
}
