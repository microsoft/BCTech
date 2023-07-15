# Business Central Performance Toolkit (BCPT) FAQ (Frequently Asked Questions)

## Can I use BCPT in production?
No, BCPT does not work on production environments.

## Does BCPT work on-premises?
Yes, BCPT work on on-premises/local sandboxes and on online sandboxes.

## Can I run scenarios in a DevOps pipeline?
Yes, it is possible to run BCPT scenario runs in AL-Go for GitHub. See https://github.com/microsoft/AL-Go/blob/main/RELEASENOTES.md#new-workflow-create-new-performance-test-app:~:text=New%20workflow%3A%20Create%20new%20Performance%20Test%20App

## Do you have sample code for scenarios?
Yes, to get you started quickly, open source scenario code is available here: https://github.com/microsoft/ALAppExtensions/tree/main/Other/Tests/BCPT-SampleTests

## What does it cost?
The Performance Toolkit is free to download and use.

## Can I get telemetry for BCPT runs?
Yes, if you have enabled telemetry on your test environment, you get telemetry when BCPT runs start/complete and when scenarios complete (including measurements). See more at [Analyzing Performance Toolkit Telemetry](https://learn.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-performance-toolkit-trace)

## How can I analyze BCPT data?
The telemetry data emitted from the Performance Toolkit can be analyzed using the Power BI Performance report:
![Performance Toolkit report in PBI](images/bcpt-pbi-report.png)

You can also analyze this data directly with KQL, see sample code here: https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/KQL/Queries/ExampleQueriesForEachArea/PerformanceToolkit.kql

## What Power Point presentations are available?
To make it easier to adopt BCPT, we added a number of Power Point presentations that you can use for various situations.

For more information, see [Powerpoint presentations](./presentations/README.md)

## I want to learn about BCPT. Where are the blogs?
Please visit the [BCPT video page](VIDEOS.md) for learning resources published as videos if you love to learn things this way.

Please visit the [BCPT blogs page](BLOGS.md) for learning resources published as blogs if blogs is your thing.


# Disclaimer
Microsoft Corporation (“Microsoft”) grants you a nonexclusive, perpetual, royalty-free right to use and modify the software code provided by us for the purposes of illustration  ("Sample Code") and to reproduce and distribute the object code form of the Sample Code, provided that you agree: (i) to not use our name, logo, or trademarks to market your software product in which the Sample Code is embedded; (ii) to include a valid copyright notice on your software product in which the Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend us and our suppliers from and against any claims or lawsuits, whether in an action of contract, tort or otherwise, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code or the use or other dealings in the Sample Code. Unless applicable law gives you more rights, Microsoft reserves all other rights not expressly granted herein, whether by implication, estoppel or otherwise. 

THE SAMPLE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL MICROSOFT OR ITS LICENSORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE SAMPLE CODE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.