error.label <- 101
error.classificationtraining <- 102
error.regressiontraining <- 103
error.trainpercent <- 104
error.insufficienttrainingdata <- 105
error.wrongpredictionmodel <- 106
error.wrongpredictionmethod <- 107
error.classificationerror <- 108
error.regressionerror <- 109
error.wrongevaluationmethod <- 110
error.regressionevaluationerror <- 111
error.insufficientevaluationdata <- 112
error.classificationevaluationerror <- 113
error.typeconversionerror <- 114
error.ploterror <- 115

type.numeric <- "decimal"
type.integer <- "integer"
type.biginteger <- "biginteger"
type.boolean <- "boolean"
prediction.converttypes <- function(data, types) {
  for (i in 1:(length(names(data)) - 1)) {
    typei <- types[[paste0("featuretype", i)]]
    featurei <- data[[paste0("feature", i)]]
    featurei <- prediction.converttype(featurei, typei)
    if (length(featurei) > 0) {
      data[[paste0("feature", i)]] <- featurei
    }
  }
  labeltype <- types$labeltype
  labels <- prediction.converttype(data$label, labeltype)
  if (length(labels) > 0) {
    data$label <- labels
  }

  return(data)
}

prediction.converttype <- function(column, typeparameter) {
  if (is.null(column) || all(is.na(column))) {
    return(column)
  }

  column <- as.character(column)

  if (is.null(typeparameter)) {
    column <- type.convert(column, as.is = TRUE)
  } else {
    typeparameter <- tolower(typeparameter)
    if (type.numeric == typeparameter || type.integer == typeparameter || type.biginteger == typeparameter) {
      column <- as.numeric(column)
    } else if (type.boolean == typeparameter) {
      column <- as.logical(column)
    } else {
      column <- as.factor(column)
    }
  }
  if (anyNA(column)) {
    prediction.throwerror(
      error.typeconversionerror,
      paste0("NA introduced when converting type to '", typeparameter, "'.")
    )
  }

  return(column)
}

prediction.getfeatures <- function(data) {
  features <- list()
  for (i in 1:(length(names(data)) - 1)) {
    if (anyNA(data[, i])) {
      break
    }
    features[[i]] <- paste0("feature", i)
  }
  return(features)
}

prediction.serializemodel <- function(model) {
  bin <- rawConnection(raw(0), open = "r+")
  saveRDS(model, file = bin)
  serializedModel <-
    c(base64Encode(rawConnectionValue(bin), "txt"))
  close(bin)

  return(serializedModel)
}

prediction.deserializemodel <- function(serializedModel) {
  tryCatch({
    bin <- base64Decode(serializedModel, mode = "raw")
    connection <- rawConnection(bin)
    model <- readRDS(connection)

    return(model)
  }, error = function(cond) {
    prediction.throwerror(
      error.wrongpredictionmodel,
      paste("Model cannot be deserialized. Error:", cond)
    )
  }, finally = {
    if (exists("connection")) {
      close(connection)
    }
  })
}

prediction.checktrainingdata <- function(data, parameters) {
  label.count <- length(unique(data$label))
  if (label.count <= 1) {
    prediction.throwerror(
      error.label,
      paste(
        "Label cannot have unique value in training. Current count:",
        label.count
      )
    )
  }

  if (!is.numeric(parameters$train_percent) ||
    parameters$train_percent <= 0 ||
    parameters$train_percent >= 1) {
    prediction.throwerror(
      error.trainpercent,
      paste(
        "Training percentage must be between 0 and 1. Current value:",
        parameters$train_percent
      )
    )
  }

  data.requiredrows <- ceiling(parameters$train_percent * 10)
  if (nrow(data) < data.requiredrows) {
    prediction.throwerror(
      error.insufficienttrainingdata,
      paste0(
        "Insufficient size of training data. Needed size: ",
        data.requiredrows,
        ". Current size: ",
        nrow(data)
      )
    )
  }
}

prediction.train <- function(data, parameters) {
  if (!is.numeric(data$label)) {
    return(prediction.trainclassification(data, parameters))
  } else {
    return(prediction.trainregression(data, parameters))
  }
}

prediction.trainclassification <- function(data, parameters) {
  prediction.checktrainingdata(data, parameters)
  tryCatch(
    {
      # Partition data into test and training data
      train.index <-
        createDataPartition(data[, "label"], p = parameters$train_percent, list = FALSE)
      data.train <- data[train.index, ]
      data.test <- data[-train.index, ]

      # Train model
      formula <- paste(
        "label ~",
        paste(prediction.getfeatures(data), collapse = " + ")
      )
      model <- rpart(formula, data = data.train, method = "class")
      environment(model$terms) <- NULL

      # Evaluate model
      quality <-
        prediction.evaluateclassification(model, data, data.test)

      # Create result with serialized model
      result <- cbind(
        model = prediction.serializemodel(model),
        quality,
        method = "classification"
      )

      return(result)
    },
    error = function(cond) {
      prediction.throwerror(
        error.classificationtraining,
        paste("Error in classification training. Error:", cond)
      )
    }
  )
}

prediction.trainregression <- function(data, parameters) {
  prediction.checktrainingdata(data, parameters)
  tryCatch(
    {
      # Partition data into test and training data
      train.index <-
        createDataPartition(data[, "label"], p = parameters$train_percent, list = FALSE)
      data.train <- data[train.index, ]
      data.test <- data[-train.index, ]

      # Train model
      formula <- paste(
        "label ~",
        paste(prediction.getfeatures(data), collapse = " + ")
      )
      model <- rpart(formula, data = data.train, method = "anova")
      environment(model$terms) <- NULL

      # Evaluate model
      quality <- prediction.evaluateregression(model, data.test)

      # Create result with serialized model
      result <- cbind(
        model = prediction.serializemodel(model),
        quality,
        method = "regression"
      )

      return(result)
    },
    error = function(cond) {
      prediction.throwerror(
        error.regressiontraining,
        paste("Error in regression training. Error:", cond)
      )
    }
  )
}

prediction.checkevaluationdata <- function(data) {
  evaluationdata.minimumrows <- 2

  if (nrow(data) < evaluationdata.minimumrows) {
    prediction.throwerror(
      error.insufficientevaluationdata,
      paste0(
        "Insufficient evaluation data. Current count: ",
        nrow(data),
        ". Needed: ",
        evaluationdata.minimumrows
      )
    )
  }
}

prediction.evaluate <- function(data, parameters) {
  prediction.checkevaluationdata(data)

  model <- prediction.deserializemodel(parameters$model)

  if (model$method == "class") {
    tryCatch(
      {
        return(prediction.evaluateclassification(model, data, data))
      },
      error = function(cond) {
        prediction.throwerror(
          error.classificationevaluationerror,
          paste(
            "Error in classification evaluation. Error:",
            cond
          )
        )
      }
    )
  } else if (model$method == "anova") {
    tryCatch(
      {
        return(prediction.evaluateregression(model, data))
      },
      error = function(cond) {
        prediction.throwerror(
          error.regressionevaluationerror,
          paste(
            "Error in regression evaluation. Error:",
            cond
          )
        )
      }
    )
  } else {
    prediction.throwerror(
      error.wrongevaluationmethod,
      paste(
        "Unknown evaluation method for model. Method:",
        model$method
      )
    )
  }
}

prediction.evaluateregression <- function(model, data.test) {
  prediction <- predict(model, data.test)
  r <- cor(prediction, data.test$label)
  if (is.na(r)) {
    r <- 0
  }
  r2 <- r * r

  return(data.frame(quality = r2, r2 = r2))
}

prediction.evaluateclassification <-
  function(model, data, data.test) {
    # The model has not learn anything it always returns one value
    if (length(model$frame$var) == 1) {
      return(
        data.frame(
          quality = 0,
          accuracy = 0,
          precision = 0,
          recall = 0,
          f1 = 0,
          balancedAccuracy = 0
        )
      )
    }

    predicted <- predict(model, data.test, type = "class")
    u <- union(predicted, data[, "label"])
    predicted <- factor(predicted, u)
    y <- factor(data.test[, "label"], u)

    if (nlevels(y) == 1) {
      positives <- y # We define given label as positive
      truepositives.count <- sum(predicted == positives)
      falsepositives.count <- 0
      truenegatives.count <- 0
      falsenegatives.count <- sum(predicted != positives)

      accuracy <-
        (truepositives.count + truenegatives.count) / length(predicted)
      precision <-
        truepositives.count / (truepositives.count + truenegatives.count)
      recall <-
        truepositives.count / (truepositives.count + falsenegatives.count)
      if (falsenegatives.count == 0) {
        balancedAccuracy <- accuracy
      } else {
        balancedAccuracy <- (truepositives.count / (truepositives.count + falsepositives.count) +
          truenegatives.count / (truenegatives.count + falsenegatives.count)) / 2
      }
    } else {
      t <- table(predicted, y)
      cm <- confusionMatrix(t)

      accuracy <- cm$overall["Accuracy"]
      if (nlevels(y) == 2) {
        precision <- cm$byClass["Pos Pred Value"]
        recall <- cm$byClass["Sensitivity"]
        balancedAccuracy <- cm$byClass["Balanced Accuracy"]
      } else {
        # change NaNs to 0
        cm$byClass[, "Pos Pred Value"][!complete.cases(cm$byClass[, "Pos Pred Value"])] <- 0
        precision <- mean(cm$byClass[, "Pos Pred Value"])
        recall <- mean(cm$byClass[, "Sensitivity"])
        balancedAccuracy <- mean(cm$byClass[, "Balanced Accuracy"])
      }
    }

    if (is.na(recall)) {
      recall <- 0
    }
    if (is.na(precision)) {
      precision <- 0
    }

    if (recall + precision == 0) {
      f1 <- 0
    } else {
      f1 <- (2 * precision * recall) / (precision + recall)
    }

    return(
      data.frame(
        quality = balancedAccuracy,
        accuracy = accuracy,
        precision = precision,
        recall = recall,
        f1 = f1,
        balancedAccuracy = balancedAccuracy
      )
    )
  }

prediction.predict <- function(data, parameters) {
  model <- prediction.deserializemodel(parameters$model)

  if (model$method == "class") {
    return(predict.predictclassification(model, data, parameters))
  } else if (model$method == "anova") {
    return(predict.predictregression(model, data, parameters))
  } else {
    prediction.throwerror(
      error.wrongpredictionmethod,
      paste(
        "Unknown prediction method for model. Method:",
        model$method
      )
    )
  }
}

predict.predictclassification <- function(model, data, parameters) {
  tryCatch(
    {
      prediction <- predict(model, data)
      maxIndexes <- apply(prediction, 1, which.max)
      labels <- colnames(prediction)[maxIndexes]

      data$label <- NULL
      data <- cbind(data, label = labels)

      confidences <- prediction[cbind(seq_len(nrow(prediction)), maxIndexes)]
      data$confidence <- NULL
      data <- cbind(data, confidence = confidences)

      return(data)
    },
    error = function(cond) {
      prediction.throwerror(
        error.classificationerror,
        paste(
          "Error in classification prediction. Error:",
          cond
        )
      )
    }
  )
}

predict.predictregression <- function(model, data, parameters) {
  tryCatch(
    {
      prediction <- predict(model, data, method = "vector")

      labels <- prediction

      data$label <- NULL
      data <- cbind(data, label = labels)

      return(data)
    },
    error = function(cond) {
      prediction.throwerror(
        error.regressionerror,
        paste(
          "Error in regression prediction. Error:",
          cond
        )
      )
    }
  )
}

prediction.replaceCaptions <- function(model, captions) {
  captions <- strsplit(captions, ",")[[1]]
  default_captions <- levels(model$frame$var)
  model$frame$var <- as.character(model$frame$var)
  for (i in 1:length(model$frame$var)) {
    if (model$frame$var[i] != "<leaf>") {
      current <- model$frame$var[i]
      index <- as.numeric(gsub("feature", "", current))
      new <- captions[index]
      xlevels <- attr(model, "xlevels")
      if (length(xlevels) > 0 && !is.null(xlevels[current][[1]])) {
        xlevels[new] <- xlevels[current]
        attr(model, "xlevels") <- xlevels
      }
      model$frame$var[i] <- new
    }
  }
  model$frame$var <- as.factor(model$frame$var)
  return(model)
}

prediction.plotmodel <- function(data, parameters) {
  library(rpart.plot)
  tryCatch(
    {
      model <- prediction.deserializemodel(parameters$model)
      captions <- parameters$captions
      classes <- parameters$labels
      if (!is.null(captions)) {
        model <- prediction.replaceCaptions(model, as.character(captions))
      }
      if (!is.null(classes)) {
        attr(model, "ylevels") <- strsplit(as.character(classes), ",")[[1]]
      }

      pdf(resultfile <- tempfile(fileext = ".pdf"))
      if (model$method == "class") {
        prp(model, extra = 8, varlen = 30, under = TRUE, roundint = FALSE)
      } else {
        prp(model, varlen = 30, roundint = FALSE)
      }
      dev.off()
      bytes <- readBin(resultfile, "raw", file.info(resultfile)$size)
      plot <- as.character(base64Encode(bytes, "text"))
      result <- as.data.frame(plot, stringsAsFactors = FALSE)
      return(result)
    },
    error = function(cond) {
      prediction.throwerror(
        error.ploterror,
        paste(
          "Error in ploting the model. Error:",
          cond
        )
      )
    }
  )
}

prediction.throwerror <- function(errorcode, message) {
  stop(paste0("Error code: ", errorcode, ". ", message))
}

smart.prediction <- function(data, parameters) {
  library(rpart)
  library(forecast)
  library(caret)
  library(RCurl) # Only for base64

  parameters <-
    lapply(lapply(parameters, as.character), type.convert, as.is = TRUE) # class: data.frame

  data <-
    prediction.converttypes(data, types = parameters) # class: data.frame

  if (parameters$method == "train") {
    output <- prediction.train(data, parameters)
  } else if (parameters$method == "trainclassification") {
    output <- prediction.trainclassification(data, parameters)
  } else if (parameters$method == "trainregression") {
    output <- prediction.trainregression(data, parameters)
  } else if (parameters$method == "predict") {
    output <- prediction.predict(data, parameters)
  } else if (parameters$method == "evaluate") {
    output <- prediction.evaluate(data, parameters)
  } else if (parameters$method == "plotmodel") {
    output <- prediction.plotmodel(data, parameters)
  } else if (parameters$method == "none") {
    # Ping operation
    output <- data.frame(ping = c("pong"))
  } else {
    prediction.throwerror(100, paste("Unknown method:", parameters$method))
  }

  return(output)
}
