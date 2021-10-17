# Imagegram
## Introduction
This repository is a RESTful API inspired by a system that allows you to upload images and comment on them. There's a mixture of application being built together in one system architecture.
## Technology stacks
1. .NET 5 - Containerized server application
2. NodeJS - Lambda function
3. MySQL  - RDBMS
## Core requirement
1. Large image size up to 100MB upload
2. Image file conversion
3. Low latency query
## System design perspective
### Containerized App only
<img src="photo/approach_1.JPG" alt="OD_1" width="500"/>
This approach is the most common microservice design to consume and handle high traffic environment.

1. Perform horizontal scaling by increasing and decreasing number of instances based on traffic
2. Good in handling large file transfer through multi-part upload
3. The image upload is synchronous where users can know the status of their upload in real time.
4. However, this system is good if there's a consistency of high traffic all the time

### Serverless (async / sync)
<img src="photo/approach_2.JPG" alt="OD_1" width="500"/>
This approach uses fully serverless application mainly lambda functions and apigateway

1.
