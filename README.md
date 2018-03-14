# GraphMed Client V 1.0
GraphMed is a project developed by Jakob Lindblad, Haris Eminovic, Nikola Pavlovic, and Eric Nilsson from Lund University for ServiceWell AB.
## Introduction
The application enables transferring the Snomed CT terminology-server into a Neo4j graph database using C#, .NET.
The Snomed CT dataset is stored in .txt-files, delimited by tabs, which is parsed through Neo4j LoadCsv.
Features also include quering the Neo4j database for specific patterns, simple delete functions, aswell as validating files.

### Motivation
The Swedish healthcare industry is currently using a large amount of different terminologies to store data which complicates the communication och merging of the different information systems. 
Snomed CT is a terminology which aims to support a standardized way of storing this data. 
Snomed CT provides the possibility for information that is documented to be communicated and 
transmitted between different information systems with maintained importance in a unique, safe, and comparable way.

The Snomed CT provides a complex pattern between healthcare phrases --Concepts--, where all Concepts have a relationship to one or moore Concepts.
Ex: Heart structure (body structure) --IS_A--> Structure of thoracic viscus (body structure).
Quering this relationships in a traditionall RDBMS comes with a perfomance issue which leads to the need for a graph database.
Neo4j graph database enables moore simple querying of complex patterns.

Our intention with this application is to support an easy way of importing the Snomed CT termonology into a Neo4j database as well as querying simple queries to show the powerful effect of using a graph database.

### Installation
#### Step 1
 * Setup and start a connection to a Neo4j Database. For information how to do it, please visit [the official Neo4j documentation](https://neo4j.com/docs/operations-manual/current/installation/). 
 * Start the GraphMed application. You can choose to enter the neo4j credentials as arguments on startup as below, or you can leave it blank and login in the next step after startup.
 ```
 [username] [password] [Neo4j URI] 
 ```
 Example: 
  ```
 MyUser MyPassword http://localhost:7474/db/data
 ```
 * You will then be redirected to the Command Prompt.
#### Step 2
 * If not logged in directly, type: 
 ```
 -Login [username]-[password]-[Neo4j URI]
 ```
 Example: 
 ```
 -Login MyUser-MyPassword-http://localhost:7474/db/data
 ```
 * You have now initialized a connection to the neo4j database.
 #### Step 3
 * Locate your paths to the Snomed CT folder and your Neo4j database folder.
 * Identify what version your want to import, e.g. "20170731".
 #### Step 4
 * In the Command Prompt type:
 ```
 -Install [Snomed CT folder path]-[Neo4j database folder path]-[Snomed CT version]
 ```
 Example: 
 ```
 -Install "C:\Users\User\Documents\SnomedCT_InternationalRF2_PRODUCTION"-"C:\Users\User\Documents\Neo4j\default.graphdb"-20170731
 ```
 * Your are now loading the entire dataset into Neo4j, indexing and constraining key values at each node. This may take several minutes.
 
 ### Arguments
 #### -Search
* The "-Search" function enables you to do simple queries to the database in a strict search pattern. 
* The function is built as follows: 
 ```
 -Search [search term]-[relatives]-[relationship steps]-[acceptability terms]-[language code]
 ```
* The "search term" is the starting node that you want to search for. It must either be a numerical "ConceptId" or a "Term".
 
* The "relatives" is the relationship you want to search for. It can either be "p" (parents; all nodes pointing at the searched node), "c" (children; all nodes which the searched node points at), or "f" (family; both parents and children).
 
* The "relationship steps" dictates how many steps away from the searched term you want to retreive. 
 
* The "acceptability terms" states what acceptability you want for the result; if you only want the preferred terms or the acceptable ones. It must either be "pref" (preferred) or "acc" (acceptable). 

* The "language code" is a two letter acronym for the language you want to search for. As of version 1.0 the only languages that are supported are GB (brittish) and american (US). 

* Hence, a search can look something like: 
 ```
 -Search Duckbill flathead-f-2-pref-GB
 ```
 Which means that you want to look for all preferred brittish terms that relates to "Duckbill flathead" two steps away. 
 
 * Another example would be: 
 ```
 -Search 117003-c-3-acc-US
 ```
 Which means that you want to look for all acceptable american terms that derives from the concept "117003" three steps away. 
 
 #### -Exit
 * The "-Exit" function exits the application.
 #### -Help
 * The "-Help" function displays a list of functions you can do in the application and how they work. 
 #### -Delete
 * The "-Delete" function deletes the entire database and all of its indexes and constraints. 
 
 
 
 
