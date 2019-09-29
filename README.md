# azureserverless
This code is a demo of a single page web app that users serverless function and static web page served directly from an azure blob 

The web app adds numbers entered. Its purpose is to demonstrate stateless coding as required by serveless functions. How it works is as follows.
1. index.html : This web page is a static page that is served directly from an azure blob. The page provides an interface to post 3 values (username, cmd, value) to a serveless function ( datareceiver.csx http triggered)
2. datareceiver.csx: This file holds the code for accepting http request. It expect 3 values username, cmd and value. The cmd can take one of 3 value. 
   1. entry : This cmd value indicates that the value is to be saved in an azure table.
   2. add : This cmd value indicates that all value entered are to be added and the result saved.
   3. answer : This cmd value indicates that the latest sum of adding the values is to be returned.
3. queueprocessor.csx: This code will be triggered by an add cmd value. When an add cmd value is received by the datareceiver, the receiver will create a queue entry consisting of the username. The queue entry will trigger this queueprocessor to run. The queueprocessor will get the last accumulated sum and add to it all values entered after the last time an add was carried out. The result will be saved in the azure table.

The steps to creating the app using this code is given below. All steps are carried out via the azure portal. As such an azure account is needed to try out the code. Instruction for creating an azure account is found on this link https://azure.microsoft.com/en-gb/free/?WT.mc_id=A261C142F

1. HTTP triggerd function. In azure portal create a azure function app by following the steps in this link https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-function-app-portal.
2. Create a http trigger function by following the steps in this link https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-first-azure-function#create-function.
3. replace the function body with the code in datareceiver.csx.
4. Add output binding to a azure table and queue following the steps on this link https://docs.microsoft.com/en-us/azure/azure-functions/functions-integrate-storage-queue-output-binding?tabs=csharp.
5. Add input binding to the azure table following the instruction on this link :https://docs.microsoft.com/en-us/azure/azure-functions/functions-triggers-bindings.
6. Create an AZURE Queue triggered function: https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-storage-queue-triggered-function.
7.Replace the code in the queue triggered function with the code in queueprocessor.csx.
8.Create a static website via an html pages served directly from an azure blob by following the intructions in this page https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blob-static-website
9. Use the index.html page provided for the static website index page.
10 request the index.html page and make an entry: by providing value for username,cmd and value

  
