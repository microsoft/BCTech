library(forecast)
library(plyr)

smart.forecast <- function(dataset1, dataset2) {
  OUTPUT_TYPE_FORECAST <- 1
  OUTPUT_TYPE_MODEL_VAR <- 2
  MAX_DECIMAL <- 79228162514264337593543950335

  # Time series models
  TIME_SERIES_MODEL_ALL <- "ALL"
  TIME_SERIES_MODEL_ARIMA <- "ARIMA"
  TIME_SERIES_MODEL_ETS <- "ETS"
  TIME_SERIES_MODEL_STL <- "STL"
  TIME_SERIES_MODEL_ETS_ARIMA <- "ETS+ARIMA"
  TIME_SERIES_MODEL_ETS_STL <- "ETS+STL"
  TIME_SERIES_MODEL_TBATS <- "TBATS"
  TIME_SERIES_MODEL_DEFAULT <- "DEFAULT"

  TIME_SERIES_MODELS <- c(TIME_SERIES_MODEL_ALL, TIME_SERIES_MODEL_ARIMA, TIME_SERIES_MODEL_ETS, TIME_SERIES_MODEL_STL, TIME_SERIES_MODEL_ETS_ARIMA, TIME_SERIES_MODEL_ETS_STL, TIME_SERIES_MODEL_TBATS, TIME_SERIES_MODEL_DEFAULT)

  # Missing value substitution options
  MISSING_VALUE_MEAN <- "MEAN"
  MISSING_VALUE_PREVIOUS <- "PREVIOUS"
  MISSING_VALUE_INTERPOLATION_LINEAR <- "INTERPOLATE LINEAR"
  MISSING_VALUE_INTERPOLATION_POLYNOMIAL <- "INTERPOLATE POLYNOMIAL"

  MISSING_VALUE_OPTIONS <- c(MISSING_VALUE_MEAN, MISSING_VALUE_PREVIOUS, MISSING_VALUE_INTERPOLATION_LINEAR, MISSING_VALUE_INTERPOLATION_POLYNOMIAL)

  dataset1$DateKey = as.numeric(dataset1$DateKey)
  data <- dataset1

  order_and_fill_missing_values <- function(data, missingValueSubstitution) {
    # Order data by date key
    data <- data[order(data$DateKey), ]

    # Get minimum and maximum date keys
    min_datekey <- head(data$DateKey, 1)
    max_datekey <- tail(data$DateKey, 1)

    # Fill records with missing date keys with NaN
    filled_data <- merge(data.frame(list(DateKey = seq(min_datekey, max_datekey))), data, all = T)

    # Replace NaN with a value, so forecast models knows that there was no data.
    if (is.numeric(missingValueSubstitution)) {
      filled_data$TransactionQty[is.na(filled_data$TransactionQty)] <- missingValueSubstitution
    } else if (missingValueSubstitution == MISSING_VALUE_MEAN) {
      filled_data$TransactionQty[is.na(filled_data$TransactionQty)] <- mean(filled_data$TransactionQty, na.rm = TRUE)
    } else if (missingValueSubstitution == MISSING_VALUE_PREVIOUS) {
      filled_data$TransactionQty <- na.locf(filled_data$TransactionQty)
    } else if (missingValueSubstitution == MISSING_VALUE_INTERPOLATION_LINEAR) {
      filled_data$TransactionQty <- na.approx(filled_data$TransactionQty)
    } else if (missingValueSubstitution == MISSING_VALUE_INTERPOLATION_POLYNOMIAL) {
      filled_data$TransactionQty <- na.spline(filled_data$TransactionQty)
    }

    return(filled_data)
  }

  mape <- function(observed, predicted) {
    observedLenght <- length(observed)

    if (observedLenght == 0) {
      return(-1)
    }

    # The same number of predicted and observed elements must be used
    predicted <- head(predicted, n = observedLenght)

    ape <- (abs(observed - predicted)) / observed * 100
    mape <- mean(ape, na.rm = TRUE)

    if (is.nan(mape)) {
      mape <- -1
    }

    return(mape)
  }

  modelArima <- function(historicalData, forecastHorizon, confidenceLevel) {
    return(tryCatch(
      {
        arima_model <- auto.arima(historicalData)
        tryCatch(
          {
            # auto.arima does not search all D = 1 models, so try this
            arima_model1 <- Arima(historicalData, seasonal = c(0, 1, 0))
            if (accuracy(arima_model1)[1, "MAPE"] < accuracy(arima_model)[1, "MAPE"]) {
              arima_model <- arima_model1
            }
          },
          error = function(err) {
            print(paste("Could not fit ARIMA D = 1, model error:  ", err))
          }
        )
        fcast <- forecast(arima_model, h = forecastHorizon, level = c(confidenceLevel))

        mean_arima <- as.numeric(fcast$mean)
        sigma_arima <- as.numeric((fcast$upper - fcast$lower) / 2)
        arima_fitted <- as.numeric(fitted(arima_model))

        return(list(mean = mean_arima, sigma = sigma_arima, fitted = arima_fitted))
      },
      error = function(err) {
        print(paste("ARIMA model error:  ", err))
        return(NULL)
      }
    ))
  }

  modelTBats <- function(historicalData, forecastHorizon, confidenceLevel) {
    return(tryCatch(
      {
        tbats_model <- tbats(historicalData)
        fcast <- forecast(tbats_model, h = forecastHorizon, level = c(confidenceLevel))

        mean_tbats <- as.numeric(fcast$mean)
        sigma_tbats <- as.numeric((fcast$upper - fcast$lower) / 2)
        tbats_fitted <- as.numeric(fitted(tbats_model))

        return(list(mean = mean_tbats, sigma = sigma_tbats, fitted = tbats_fitted))
      },
      error = function(err) {
        print(paste("TBats model error:  ", err))
        return(NULL)
      }
    ))
  }

  modelEts <- function(historicalData, forecastHorizon, confidenceLevel) {
    return(tryCatch(
      {
        ets_model <- ets(historicalData)
        fcast <- forecast(ets_model, h = forecastHorizon, level = c(confidenceLevel))

        mean_ets <- as.numeric(fcast$mean)
        sigma_ets <- as.numeric((fcast$upper - fcast$lower) / 2)
        ets_fitted <- as.numeric(fitted(ets_model))

        return(list(mean = mean_ets, sigma = sigma_ets, fitted = ets_fitted))
      },
      error = function(err) {
        print(paste("ETS model error:  ", err))
        return(NULL)
      }
    ))
  }

  modelStl <- function(historicalData, forecastHorizon, confidenceLevel) {
    if ((seasonality > 1) && (length(historicalData) > 2 * seasonality)) {
      tryCatch(
        {
          fcast <- stlf(historicalData, h = forecastHorizon, level = c(confidenceLevel))

          mean_stl <- as.numeric(fcast$mean)
          sigma_stl <- as.numeric((fcast$upper - fcast$lower) / 2)
          stl_fitted <- as.numeric(fitted(fcast))

          return(list(mean = mean_stl, sigma = sigma_stl, fitted = stl_fitted))
        },
        error = function(err) {
          print(paste("STL model error:  ", err))
          return(NULL)
        }
      )
    } else {
      return(NULL)
    }
  }

  modelEtsArima <- function(ets_result, arima_result) {
    if (!is.null(ets_result) && !is.null(arima_result)) {
      mean_ets_arima <- (arima_result$mean + ets_result$mean) / 2.0
      sigma_ets_arima <- sqrt(0.25 * ets_result$sigma * ets_result$sigma + 0.25 * arima_result$sigma * arima_result$sigma)
      ets_arima_fitted <- (arima_result$fitted + ets_result$fitted) / 2.0

      return(list(mean = mean_ets_arima, sigma = sigma_ets_arima, fitted = ets_arima_fitted))
    }
    return(NULL)
  }

  modelEtsStl <- function(ets_result, stl_result) {
    if (!is.null(ets_result) && !is.null(stl_result)) {
      mean_ets_stl <- (stl_result$mean + ets_result$mean) / 2.0
      sigma_ets_stl <- sqrt(0.25 * ets_result$sigma * ets_result$sigma + 0.25 * stl_result$sigma * stl_result$sigma)

      ets_stl_fitted <- (stl_result$fitted + ets_result$fitted) / 2.0

      return(list(mean = mean_ets_stl, sigma = sigma_ets_stl, fitted = ets_stl_fitted))
    }
    return(NULL)
  }

  addToOutputForecast <- function(forecast_data_frame, type, granularity_attribute, date_key, transaction_qty, sigma) {
    new_data_frame <- data.frame(Type = type, GranularityAttribute = granularity_attribute, DateKey = date_key, TransactionQty = transaction_qty, Sigma = sigma)
    return(rbind(forecast_data_frame, new_data_frame))
  }

  addToOutputVars <- function(vars_data_frame, type, granularity_attribute, error_percentage, forecast_model_name) {
    new_data_frame <- data.frame(Type = type, GranularityAttribute = granularity_attribute, ErrorPercentage = error_percentage, ForecastModelName = forecast_model_name)
    return(rbind(vars_data_frame, new_data_frame))
  }

  trainModels <- function(timeSeriesModel, historicalData, forecastHorizon, confidenceLevel) {
    result <- list()

    # ARIMA
    if (timeSeriesModel == TIME_SERIES_MODEL_ARIMA || timeSeriesModel == TIME_SERIES_MODEL_ETS_ARIMA || timeSeriesModel == TIME_SERIES_MODEL_ALL) {
      arimaForecast <- modelArima(historicalData, forecastHorizon, confidenceLevel)

      if (timeSeriesModel == TIME_SERIES_MODEL_ARIMA || timeSeriesModel == TIME_SERIES_MODEL_ALL) {
        result[[TIME_SERIES_MODEL_ARIMA]] <- arimaForecast
      }
    }

    # TBATS
    if (timeSeriesModel == TIME_SERIES_MODEL_TBATS || timeSeriesModel == TIME_SERIES_MODEL_ALL || timeSeriesModel == TIME_SERIES_MODEL_DEFAULT) {
      tbatsForecast <- modelTBats(historicalData, forecastHorizon, confidenceLevel)

      result[[TIME_SERIES_MODEL_TBATS]] <- tbatsForecast
    }

    # ETS
    if (timeSeriesModel == TIME_SERIES_MODEL_ETS || timeSeriesModel == TIME_SERIES_MODEL_ETS_ARIMA || timeSeriesModel == TIME_SERIES_MODEL_ETS_STL || timeSeriesModel == TIME_SERIES_MODEL_ALL) {
      etsForecast <- modelEts(historicalData, forecastHorizon, confidenceLevel)

      if (timeSeriesModel == TIME_SERIES_MODEL_ETS || timeSeriesModel == TIME_SERIES_MODEL_ALL) {
        result[[TIME_SERIES_MODEL_ETS]] <- etsForecast
      }
    }

    # STL
    if (timeSeriesModel == TIME_SERIES_MODEL_STL || timeSeriesModel == TIME_SERIES_MODEL_ETS_STL || timeSeriesModel == TIME_SERIES_MODEL_ALL) {
      stlForecast <- modelStl(historicalData, forecastHorizon, confidenceLevel)

      if (timeSeriesModel == TIME_SERIES_MODEL_STL || timeSeriesModel == TIME_SERIES_MODEL_ALL) {
        result[[TIME_SERIES_MODEL_STL]] <- stlForecast
      }
    }

    # ETS + ARIMA
    if (timeSeriesModel == TIME_SERIES_MODEL_ETS_ARIMA || timeSeriesModel == TIME_SERIES_MODEL_ALL) {
      result[[TIME_SERIES_MODEL_ETS_ARIMA]] <- modelEtsArima(etsForecast, arimaForecast)
    }

    # ETS + STL
    if (timeSeriesModel == TIME_SERIES_MODEL_ETS_STL || timeSeriesModel == TIME_SERIES_MODEL_ALL) {
      result[[TIME_SERIES_MODEL_ETS_STL]] <- modelEtsStl(etsForecast, stlForecast)
    }

    return(result)
  }

  ################### PARAMETERS INIT ###################

  # Convert column names to upper
  for (i in 1:length(names(dataset2))) {
    colnames(dataset2)[i] <- toupper(colnames(dataset2)[i])
  }

  # Number of forecast predictions
  horizon <- as.numeric(dataset2$HORIZON)

  if (is.na(horizon) || horizon <= 0) {
    stop(paste("Parameter HORIZON must be greater than 0."))
  }

  # Seasonality of historical data
  if (is.null(dataset2$SEASONALITY)) {
    seasonality <- 1
  } else {
    seasonality <- as.numeric(dataset2$SEASONALITY)
  }

  if (is.na(seasonality) || seasonality <= 0) {
    stop(paste("Parameter SEASONALITY must be greater than 0."))
  }

  # Start date key from which forecast should be generated
  global_max_datekey <- max(dataset1$DateKey)
  if (is.null(dataset2$FORECAST_START_DATEKEY)) {
    forecast_start_datekey <- global_max_datekey + 1
  } else {
    forecast_start_datekey <- as.numeric(dataset2$FORECAST_START_DATEKEY)

    if (forecast_start_datekey <= global_max_datekey) {
      stop(paste("Parameter FORECAST_START_DATEKEY must be greater than maximum date key of historical data (", global_max_datekey, ")"))
    }
  }

  # Time series model
  if (is.null(dataset2$TIME_SERIES_MODEL)) {
    timeSeriesModel <- TIME_SERIES_MODEL_DEFAULT
  } else {
    timeSeriesModel <- toupper(dataset2$TIME_SERIES_MODEL)
  }

  if (is.na(match(timeSeriesModel, TIME_SERIES_MODELS))) {
    stop(paste("Parameter value of TIME_SERIES_MODEL does not represent any known forecasting model."))
  }

  # Confidence level
  if (is.null(dataset2$CONFIDENCE_LEVEL)) {
    confidenceLevel <- 95
  } else {
    confidenceLevel <- as.numeric(dataset2$CONFIDENCE_LEVEL)
  }

  if (is.na(confidenceLevel) || confidenceLevel <= 0 || confidenceLevel >= 100) {
    stop(paste("Parameter CONFIDENCE_LEVEL must be greater than 0 and lower than 100."))
  }

  # Size of a test set in a percent of a total historical data size
  if (is.null(dataset2$TEST_SET_SIZE_PERCENT)) {
    testSetSizePercent <- 20
  } else {
    testSetSizePercent <- as.numeric(dataset2$TEST_SET_SIZE_PERCENT)
  }

  if (is.na(testSetSizePercent) || testSetSizePercent < 0 || testSetSizePercent >= 100) {
    stop(paste("Parameter TEST_SET_SIZE_PERCENT must be greater than or equal to 0 and lower than 100."))
  }

  # How gaps in historical data are filled
  if (is.null(dataset2$MISSING_VALUE_SUBSTITUTION)) {
    missingValueSubstitution <- 0
  } else {
    if (is.numeric(dataset2$MISSING_VALUE_SUBSTITUTION)) {
      missingValueSubstitution <- as.numeric(dataset2$MISSING_VALUE_SUBSTITUTION)
    } else {
      missingValueSubstitution <- toupper(dataset2$MISSING_VALUE_SUBSTITUTION)

      if (is.na(match(missingValueSubstitution, MISSING_VALUE_OPTIONS))) {
        stop(paste("Parameter value of MISSING_VALUE_SUBSTITUTION does not represent any known substitution option."))
      }
    }
  }

  ################### END PARAMETERS INIT ###################

  output_forecast <- data.frame(Type = numeric(0), GranularityAttribute = character(0), DateKey = numeric(0), TransactionQty = numeric(0), Sigma = numeric(0))
  output_vars <- data.frame(Type = numeric(0), GranularityAttribute = character(0), ErrorPercentage = numeric(0), ForecastModelName = character(0))

  granularityAttributes <- unique(dataset1$GranularityAttribute)
  granularityAttributes_num <- length(granularityAttributes)

  for (i in 1:granularityAttributes_num) {
    # Prepare training and test data for given granularity attribute
    if (is.na(granularityAttributes[i])) {
      granularityAttribute_data <- data[which(is.na(data$GranularityAttribute)), ]
    }
    else {
      granularityAttribute_data <- data[which(data$GranularityAttribute == granularityAttributes[i]), ]
    }

    if (nrow(granularityAttribute_data) == 0) {
      next
    }

    granularityAttribute_data <- order_and_fill_missing_values(granularityAttribute_data, missingValueSubstitution)

    full_data <- as.numeric(granularityAttribute_data$TransactionQty)
    full_data_ts <- ts(full_data, frequency = seasonality)
    full_data_last_datekey <- granularityAttribute_data$DateKey[length(granularityAttribute_data$DateKey)]
    full_data_horizon_offset <- forecast_start_datekey - full_data_last_datekey + horizon - 1

    test_size <- floor(length(granularityAttribute_data$TransactionQty) * testSetSizePercent / 100)
    resultTimeSeriesModel <- timeSeriesModel

    if (test_size > 0) {
      # When there is data in the test set then the model will be chosen based on how well it predicts the demand in the test set

      test_data <- tail(as.numeric(granularityAttribute_data$TransactionQty), n = test_size)
      train_data <- head(granularityAttribute_data$TransactionQty, n = -test_size)

      train_data_ts <- ts(train_data, frequency = seasonality)

      result <- trainModels(timeSeriesModel, train_data_ts, forecastHorizon = test_size, confidenceLevel = confidenceLevel)

      # Calculate accuracy metrics

      # Mape cannot be calculated when there are zeros in the data
      test_data_na <- replace(test_data, test_data == 0, NaN)

      result_mape <- Inf

      for (name in names(result))
      {
        # How big is the error between forecasted demand and actual demand
        current_mape <- mape(test_data_na, result[[name]]$mean)

        if (current_mape < result_mape) {
          resultTimeSeriesModel <- toupper(name)
          result_mape <- current_mape
        }
      }

      result <- trainModels(resultTimeSeriesModel, full_data_ts, full_data_horizon_offset, confidenceLevel)
    } else {
      # There is no data in the test set so the model will be chosen based on how well it fits historical data

      result <- trainModels(timeSeriesModel, full_data_ts, full_data_horizon_offset, confidenceLevel)

      # Mape cannot be calculated when there are zeros in the data
      full_data_na <- replace(full_data, full_data == 0, NaN)

      # MAPE cannot be calculated when the test set is empty
      result_mape <- -1
      min_mape_fitted <- Inf

      for (name in names(result))
      {
        # How well the model fits historical data
        current_mape_fitted <- mape(full_data_na, result[[name]]$fitted)

        if (current_mape_fitted < min_mape_fitted) {
          resultTimeSeriesModel <- toupper(name)
          min_mape_fitted <- current_mape_fitted
        }
      }
    }

    # Save result to output
    mean_aml <- c()
    sigma_aml <- c()
    mape_aml <- c()

    if (!is.null(result[[resultTimeSeriesModel]])) {
      mean_aml <- result[[resultTimeSeriesModel]]$mean
      sigma_aml <- result[[resultTimeSeriesModel]]$sigma
      mape_aml <- result_mape
    }

    if (length(mean_aml) == 0) {
      paste("Forecast cannot be generated for ", granularityAttributes[i])

      output_vars <- addToOutputVars(output_vars, OUTPUT_TYPE_MODEL_VAR, granularityAttributes[i], -1, "")
    } else {
      sigma_aml[is.infinite(sigma_aml)] <- MAX_DECIMAL

      # Forecasted values will be sent starting from forecast start date key parameter value
      output_forecast <- addToOutputForecast(output_forecast, OUTPUT_TYPE_FORECAST, granularityAttributes[i], seq(from = forecast_start_datekey, length.out = horizon), tail(as.numeric(mean_aml), horizon), tail(as.numeric(sigma_aml), horizon))
      output_vars <- addToOutputVars(output_vars, OUTPUT_TYPE_MODEL_VAR, granularityAttributes[i], mape_aml, resultTimeSeriesModel)
    }
  }

  output <- rbind.fill(output_forecast, output_vars)
  return(output)
}
