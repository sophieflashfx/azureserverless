# azureserverless
This code is a demo of a single page web app that users serverless function and static web page served directly from an azure blob 

The web app adds numbers entered. Its purpose is to demonstrate stateless coding as required by serveless functions. How it works is as follows.
1. index.html : This web page is a static page that is served directly from an azure blob. The page provides an interface to post 3 values (username, cmd, value) to a serveless function ( datareceiver.csx http triggered)
2. datareceiver.csx: This file holds the code for accepting http request. It expect 3 values username, cmd and value. The cmd can take one of 3 value. 
   1. entry : This cmd vale indicates that the value is to be saved in an azure table.
   2. add : This cms value indicates that all value entered are to be added and the result saved.
   3. answer : This cmd value indicates that the latest sum of adding the values is to be returned.
3. queueprocessor.csx: This code will be triggered by an add cmd value. When an add cmd value is received by the datareceiver, the receiver will create a queue entry consisting of the username. The queue entry will trigger this queueprocessor to run. The queueprocessor will get the last accumulated sum and add to it all values entered after the last time an add was carried out. The result will be saved in the azure table.

  
