# TODOManager
WIP

CosmosDb setup:
For the API to work properly a CosmosDb endpoint should be specified.
The endpoint URI and MasterKey should be provided in appsettings.json

Db and Container:
The Db should contain a Database and a Container with the id "Todo", and the "Todo" Container's Partition key should be "User". 
The User was chosen as the partition key, because most of the operations we do are specific to the user's TODOs, and therefor using this partitionKey will minimize crosspartition queries.

Consistency:
For Consistency level I think the "Session" consistency should be adequate, this kind of application is not time critical or need very strong consistency, so I did not consider "Strong" or "Bounded Staleness",
"Eventual consistency" is not enough and since most of my operation's are bound to a user session I would assume that the "Consistent prefix" would be also inadequate as read operations would not necesseraly get the latest changes done in the session, and I assume for this kind of application, that is a requirement.

Provisioning:
For provisioning I used manual provisioning with 400 RUs/sec, this is enough for a minimal development db instance but for production, the provisioning should be based on calculations through loadtesting.
I have introduced logging of Request Charges on the Debug level, this can help determine the RU provisioning. Alternatively, CosmosDb provides a calculator that can be used based on estimating a sample workload.
The NonFunctional requiremets did not explicitly state if the application's userbase is worldwide or not. If the application is used worldwide, I would assume that throuput is on a static level throughout the day, in this case we should use Manual provisioning.
If the application is used in a specific region, there may be different levels of throuputs in different times of the day, in this case autoscale can be helpful to scale down when the load is smaller.

Indexing:
I have managed to get smaller RU costs through excluding the Description from indexing in the database as we never use that property for queries. The cost change is minor, but given the "large number of users" requirement, the gain can be significant.