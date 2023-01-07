# What is the the intented usage of the API query generator?
The idea of these samples is to split the responsibility of the generation of APIs between teams working on Business Intelligence (BI) projects and teams knowledgable in AL development. With these two programs, the BI team can configure the APIs they need in csv files (that can be placed under version control). They can then generate the AL code for API queries they need to expose data from Business Central as OData endpoints. The AL team just needs to help them package the generated AL files into a per-tenant extension and deploy it to the appropriate Business Central environments. Hopefully, the AL team will help automate this into DevOps pipelines to completely unblock the BI team.

# What is the API generator sample?
This sample code consists of two commandline programs
1. PayloadGenerator
2. APIQueryGenerator

# PayloadGenerator
The first program _PayloadGenerator_ takes arguments to define general metadata for the API queries you want to generate as well as input files in the simplest format possible (csv) for the AL tables and fields that you would like to include in your API queries. 

This is an example input file for tables:
```
tableName
Customer
Vendor
test1
test2
test3
```

This is an example input file for fields:
```
tableName;fieldName
Customer;cfield1
Customer;cfield2
Customer;cfield3
Customer;SystemCreatedOn
Vendor;vfield1
Vendor;vfield2
Vendor;vfield3
```

The program outputs an xml file that the second program (APIQueryGenerator) needs for generating AL files containing code for API queries.

```
PayloadGenerator.exe
Usage: PayloadGenerator [OPTIONS]+

Options:
      --apipublisher=VALUE   APIPublisher (required)
      --apigroup=VALUE       APIGroup (required)
      --apiversion=VALUE     APIPublisher (required)
  -h, --help                 show this message and exit
      --inputtablefile=VALUE input csv file for tables (required)
      --inputfieldfile=VALUE input csv file for fields (required)
      --outputdir=VALUE      output directory
      --outputfile=VALUE     output file (required)
      --idnumberstart=VALUE  Id numbers for AL objects starts at this number (
                               default is 50000)
```



This is an example output file from the  _PayloadGenerator_ program:
```
<?xml version="1.0" encoding="utf-8" ?>
<APIQueries APIPublisher="Contoso" APIGroup="datawarehouse" APIVersion="1.0">
  <query id ="50000" name="QueryForTable1" filename="50000_QueryForTable1" Caption="customers" Locked="true" EntityName="Customer" EntitySetName="Customers" DataItemName="Customer" TableName ="Customers">
    <field DataItemFieldName="customerId" FieldName="Id" Caption="Customer Id" Locked="true"/>
    <field DataItemFieldName="customerNumber" FieldName="No." Caption="No" Locked="true"/>
    <field DataItemFieldName="customerName" FieldName="name" Caption="Customer Name" Locked="true"/>
  </query>
  <query id ="50001" name="QueryForTable2" filename="50001_QueryForTable2" Caption="vendors" Locked="false" EntityName="Vendor" EntitySetName="Vendors" DataItemName="Vendor" TableName ="Vendors" >
    <field DataItemFieldName="vendorId" FieldName="Id" Caption="Vendor Id" Locked="true"/>
    <field DataItemFieldName="vendorNumber" FieldName="No." Caption="No" Locked="true"/>
    <field DataItemFieldName="vendorName" FieldName="name" Caption="Vendor Name" Locked="true"/>
  </query>
</APIQueries>
```

# APIQueryGenerator
The second program _APIQueryGenerator_ takes arguments to define general metadata for the API queries you want to generate as well as input files in the simplest format possible (csv) for the AL tables and fields that you would like to include in your API queries. 

The program outputs a directory with .al files containing code for API queries - one file per API query.

```
APIQueryGenerator.exe
Usage: APIQueryGenerator [OPTIONS]+

Options:
  -h, --help                 show this message and exit
  -i, --inputfile=VALUE      input xml file (required)
  -o, --outputdir=VALUE      output directory (if not specified, then input
                               file name will be used)
  -s, --nosystemfields       Do not add system fields to API queries (default
                               is that they will be added)
```


# Disclaimer
Microsoft Corporation (“Microsoft”) grants you a nonexclusive, perpetual, royalty-free right to use and modify the software code provided by us for the purposes of illustration  ("Sample Code") and to reproduce and distribute the object code form of the Sample Code, provided that you agree: (i) to not use our name, logo, or trademarks to market your software product in which the Sample Code is embedded; (ii) to include a valid copyright notice on your software product in which the Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend us and our suppliers from and against any claims or lawsuits, whether in an action of contract, tort or otherwise, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code or the use or other dealings in the Sample Code. Unless applicable law gives you more rights, Microsoft reserves all other rights not expressly granted herein, whether by implication, estoppel or otherwise. 

THE SAMPLE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL MICROSOFT OR ITS LICENSORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE SAMPLE CODE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
