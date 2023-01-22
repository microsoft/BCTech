# Overview

To simplify the labor-intensive process of monitoring health and uptake of application features, we have introduced the `Telemetry` AL module. There are multiple benefits of using the module compared to sending telemetry via `Session.LogMessage`.
- Different features can be compared across the same metrics.
- Common information is sent together with every feature telemetry message, which allows for advanced filtering capabilities.

# How to use
> **_NOTE:_**  `Telemetry` and `Feature Telemetry` codeunits will only work if there is an implementation of the `Telemetry Logger` interface in any extension from your publisher (see [example](Uptake%20sample%20extension/MyTelemetryLogger.Codeunit.al)).

Using the `Feature Telemetry` codeunit is extremely easy. For example, to register the usage of a feature it's enough to use the following method:
```
FeatureTelemetry.LogUsage(<tag>, <feature name>, <event name>);
```

After the telemetry is emitted, the data can be aggregated and displayed (for example, using FeatureUsage Power BI report).

![FeatureUsage report](FeatureUsageReport.png)

## Description
There are 3 different kinds of events that a feature can log through the `Feature Telemetry` codeunit.
```
FeatureTelemetry.<LogUsage|LogError|LogUptake>(...)
```

- The `LogUsage` method should be called when the feature is successfully used by a user.
- `LogError` should be called when the error needs to be explicitly sent to telemetry (e. g. after a call to a try function or `Codeunit.Run` returned false, sending an http response error message etc).
- `LogUptake` should be called at the points when a user changes the uptake state of a feature. There are 4 predefined uptake states that a feature can go through: `Undiscovered`, `Discovered`, `Set up` and `Used` (note that in order to track the uptake status of a feature it may make database transactions). Calling `LogUptake` with uptake state `Undiscovered` resets the uptake state of the feature. The telemetry from this call will be used to calculate the values in the uptake funnel of the feature. 

## Guidelines

### Logging uptake
- If a feature logs uptake, then there should be calls to register all the following states: `Discovered`, `Set up`, `Used`, `Undiscovered`.
- The current convention for registering uptake states is the following.
    - State `Discovered` should be registered when pages related to the given feature are opened (or otherwise when a user _intentionally_ seeks information about a feature)
    - State `Set up` should be registered when the user performed a set up for the feature (usually right after a record in a table related to the feature is added or updated)
    - State `Used` should be registered when a user _attempts_ to use the feature (note the difference with `LogUsage`, which should be called only if the feature is used _successfully_)
    - State `Undiscovered` is optional. When a feature is unregistered you can use this option (or when a user disable it when it has been enable. Example is usage of Retention Policies)
- If `LogUptake` is called from within a try function, the parameter `PerformWriteTransactionsInASeparateSession` should be set to `true`.


### Feature and event naming
- The feature names should be well-identifiable and short (e. g. `Retention policies`, `Configuration packages`, `Emailing`)
- The event names should specify the scenario being executed. If `LogUsage` is called, past tense should be used for the event name, as the event has already happened (e. g. `Email sent`, `Retention policy applied`). If `LogError` is called, present tense should be used (e. g. `Sending email`, `Loading template`).


## FAQ
Q: Do I need to call all the functions from the `Feature Telemetry` codeunit in order for my feature to show up on the sample report?  
A: No, different telemetry calls correspond to different visualizations on the report (e. g. calling `LogUsage` ensures that MAU and MAT are computed, using `LogUptake` corresponds to the uptake funnel), however it is a good practice to have at least one `LogUsage` call in your feature.

Q: When should I use feature telemetry, and when should I use `Session.LogMessage`?  
A: `Feature Telemetry` should only be used in very specific well-defined places (see description, and guidelines), in all the other places `Session.LogMessage` should still be used.

Q: What is the performance impact of using the module?  
A: All the methods in the `Feature Telemetry` codeunit collect additional common custom dimensions with each call, so there is a small cost to using the module, but it's not significant.

Q: Do I need to control how often the uptake telemetry is emitted for my feature (e. g. to prevent the "Used" part of the funnel to be bigger than "Discovered")?  
A: No, the logic in the module will make sure that the funnel is constructed correctly.

Q: Why do we need both `LogUptake(..., Enum::"Feature Uptake State"::Used)` and `LogUsage`?  
A: Not every feature can have un uptake funnel. Such features should only call `LogUsage` (and potentially `LogError`). Therefore the usage telemetry is completely separated from uptake telemetry. Additionally, `LogUptake(..., Enum::"Feature Uptake State"::Used)` and `LogUsage` should be called from slightly different places (see guidelines above).

Q: How do common custom dimensions work? Will other extension publishers see the information I add to common custom dimensions?  
A: The common custom dimensions are aggregated _per publisher_, there will be no sharing of telemetry data between different publishers.  

# KQL query
Feature telemetry can be queried in Application Insights using the following KQL query: [FeatureTelemetry.kql](../../KQL/Queries/ExampleQueriesForEachArea/FeatureTelemetry.kql)

# Disclaimer
Microsoft Corporation (“Microsoft”) grants you a nonexclusive, perpetual, royalty-free right to use and modify the software code provided by us for the purposes of illustration  ("Sample Code") and to reproduce and distribute the object code form of the Sample Code, provided that you agree: (i) to not use our name, logo, or trademarks to market your software product in which the Sample Code is embedded; (ii) to include a valid copyright notice on your software product in which the Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend us and our suppliers from and against any claims or lawsuits, whether in an action of contract, tort or otherwise, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code or the use or other dealings in the Sample Code. Unless applicable law gives you more rights, Microsoft reserves all other rights not expressly granted herein, whether by implication, estoppel or otherwise. 

THE SAMPLE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL MICROSOFT OR ITS LICENSORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE SAMPLE CODE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
