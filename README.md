# GraphMed Client V.1.0
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
Step 1
 * Setup and start a connection to a Neo4j Database. For information how to do it, please visit [the official Neo4j documentation] (https://neo4j.com/docs/operations-manual/current/installation/). 
 * Start the GraphMed application. You can choose to enter the neo4j credentials as arguments on startup as [username] [password] [No4j URI] or leave it blank. You will then be redirected to the Command Prompt.
Step 2
 * If not logged in directly, type 
 ```
 -Login [username]-[password]-[Neo4j URI]
 ```
 Example: 
 ```
 -Login MyUser-MyPassword-http://localhost:7474/db/data
 ```
 * You have now initialized a connection to the neo4j database.
 Step 3
 * Locate your path to Snomed CT folder and your Neo4j database folder.
 * Example: Snomed CT folder:D:\SnomedCT_InternationalRF2_PRODUCTION_20170731T150000Z
			Neo4j database folder: C:\Users\Jakob\Documents\Neo4j\default.graphdb
 * Identify what version your want to import, Example: 20170731
 * NOTE: If you dont want to load the enitire dataset at once, skip step 4 and go to ARGUMENTS.
 Step 4
 * In Command Prompt type:-I:[Snomed CT folder path] [Neo4j database folder path] [Snomed CT version]
 * Your are now loading the entire dataset into Neo4j, including index and constraints. This may take several minutes.
