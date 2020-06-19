// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------------

// Fill in  your azure iot device scope id, device id and primary or secondary device key
#define SCOPE_ID    ""
#define DEVICE_ID   ""
#define DEVICE_KEY  ""

// define the name of the setting for frequency of emitting telemetry in ms and its default value (cannot be less than sensor minimum delay between readings)
#define EMIT_FREQ_SETTING "emit_freq"
#define EMIT_FREQUENCY  10000

// define the field names for your sensors here
#define TEMPERATURE "temperature"
#define HUMIDITY    "humidity"  
#define WEIGHT      "weight"
