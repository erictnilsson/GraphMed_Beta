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
#### Step 2
 * If not logged in directly, type: 
 ```
 -Login [username]-[password]-[Neo4j URI]
 ```
 Example: 
 ```
 -Login MyUser-MyPassword-http://localhost:7474/db/data
 ```
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
 ### Functions
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
 
 ## For Developers
Writing Cyphers to the database is done by the static class Cypher.cs. The Cypher handler can call the following functions with a result limit as a parameter:
```
Cypher.Create(int? limit)
```
```
Cypher.Load(int? limit, int? commit)
```
```
Cypher.Delete(int? limit?)
```
```
Cypher.Drop(int? limit?)
```
```
Cypher.Get(int? limit?)
```
### Create
```
Cypher.Create().Index<Node>(string identifier);
```
Creates an index for the specified node-type on the specified identifier.  
```
Cypher.Create().Constraint<Node>(string identifer); 
```
Creates a constraint for the specified node-type on the specified identifier.  

### Load
The Load-function is based on Cyphers LOADCSV. Apart from the limit parameter, Load also contains the commit parameter. This means that when you call Load, you need to specify how many rows in the .CSV file you want to import and the size of the periodic commit. The parameters are nullable if you don't want to set a limit or use the default commit size. The function can in turn call on the different Nodes that you can build the graph database with:
```
Cypher.Load(limit: null, commit: 20_000).Concepts(constrain: true); 
```
When loading the Concepts, you have the choice of constraining the Concepts by it's Id; speeding up the search function of the database. 
```
Cypher.Load(limit: null, commit: 20_000).Descriptions(forceRelationship: true, index: true, constrain: true); 
```
When loading the Descriptions, you have the choice of forcing a relationship on the anchoring Concept. If you do, the relationship will be made between the Description and Concept "on create". You can also choose to index and uniquely constrain the Descriptions on it's Id and ConceptId; speeding up the search function of the database. 
```
Cypher.Load(limit: null, commit: 20_000).Relationships(); 
```
```
Cypher.Load(limit: null, commit: 20_000).Refset(index: true); 
```
When loading the Refset, you have the choice of indexing the Terms by it's Term; speeding up the search function of the database. 

### Delete
```
Cypher.Delete(limit: null).All(detach: true); 
```
When deleting "all", the nodes are deleted independent on the node-type. You have the choice of detaching the delete aswell, meaning that you can detach the relationships connected to the deleted node. 
```
Cypher.Delete(limit: null).Node<Node>(detach: true);
```
When deleting "node", the nodes are deleted dependent on the specified node-type. ou have the choice of detaching the delete aswell, meaning that you can detach the relationships connected to the deleted node. 
```
Cypher.Delete(limit: null).Everything(); 
```
When deleting "everything", you detach-delete everything in the database in batches, also dropping all indexes and constraints present in the database. 
### Drop
```
Cypher.Drop().Index<Node>(); 
```
Drops the indexes on the specified node-type. 
```
Cypher.Drop().Constraint<Node>(); 
```
Drops the constraints on the specified node-type. 
### Get
```
Cypher.Get().Nodes(string searchTerm, string relatives, string limit, string acceptability, string langCode); 
```
Gets a Result-object with the specified search-pattern. 
 ## Authors
 * **Jakob Lindblad**- Project Manager
 * **Eric Nilsson**- Lead Developer
 * **Haris Eminovic**- Manager and developer of the theoretical- and conceptual framework
 * **Nikola Pavlovic**- Manager and developer of the theoretical- and conceptual framework
 
 ## Acknowledgments
 * **ServiceWell AB**- for hosting us and letting us do our internship there. 
 * **Lunds University**- 
 * **[Readify/Neo4jClient](https://github.com/Readify/Neo4jClient)**- for building a A .NET client for neo4j which is the foundation of this project. 
 
