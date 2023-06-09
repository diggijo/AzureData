# OPC Data Visualization

## Features

- Connects to an OPC server using the Unity OPC Client library.
- Retrieves data from the OPC server and sends it to an Azure Function API.
- Periodically updates the data and displays it in a graph using Unity UI components.
- Supports customization of the graph's appearance, including axis labels and grid lines.

## 1. Introduction

The OPC Data Visualization project aims to retrieve real-time data from an OPC server, send it to an Azure Function API, and visualize it in a graph format using Unity Engine. The project utilizes the Unity OPC Client library to connect to the OPC server, retrieves data at regular intervals from blob storage, and displays it dynamically in a graph.

## 2. Architecture Overview

The project consists of four main classes:

GetData: This class is responsible for retrieving data from the Blob Storage and sending it to the Azure Function API. It periodically updates the data and manages the data dictionary for graph visualization.
Dictionary_Graph: This class handles the graph visualization in Unity. It creates the graph container, labels, and data points, and provides functionality for customizing the graph appearance.
Program (in the OPC2 namespace): This class connects to the OPC server using the OPC Client library. It reads data from the OPC server, converts it to the desired format, and sends it to the Azure Function API.
HttpClient: This class handles the HTTP communication with the Azure Function API, sending the OPC data for storage and further processing.

## 3. Implementation Details

The project utilizes the Unity Engine for visualization and the OPC Client library for OPC server communication. The OPC server is specified using the UrlBuilder.Build method, and the desired OPC items are defined using OpcDaItemDefinition.

The GetData class is responsible for periodically updating the data by making HTTP GET requests to the Azure Function API. The received data is then parsed and stored in a dictionary (dataTable). The createList method converts the received data into a format compatible with the graph visualization class.

The Dictionary_Graph class handles the graph visualization. It creates the necessary UI components, such as circles for data points, labels for axis values, and dashes for grid lines. The showGraph method takes the data dictionary, customizes the graph appearance, and populates the graph with the retrieved data.

The Program class connects to the OPC server using the OPC Client library. It creates an OPC group and adds OPC items for reading data. The retrieved data is converted to the desired format, and each value is sent to the Azure Function API using the SendValueToServerless method. This method uses an HttpClient to make an HTTP POST request to the Azure Function API endpoint, sending the data in JSON format.
