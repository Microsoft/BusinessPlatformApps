Facebook Solution Template Documentation
===========================================================

# Table of Contents
1. [Introduction](#introduction)


### Introduction

The Facebook template provides brand and campaign management for Facebook pages, The template stands up an end-to-end solution that queries the Facebook Graph API, enriches the data using machine learning and stores these enrichments in Azure SQL. Users can then use pre-built Power BI reports that leverage Microsoft research technology to start exploring the data and finding insights.
The template is aimed at anyone who is interesting in following or tracking posts against a Facebook page.
The following document provides a walkthrough of the architecture, a deep dive into every component, comments on customizability as well as information on additional topics like estimated costs. For any questions not covered in this document, please contact the team at <PBISolnTemplates@microsoft.com>


The flow of the Facebook solution template is as follows:

-   Logic Apps insert dates onto the queue.

-   Azure Function picks up the dates and calls the facebook API

-   Azure function enriches the data and writes it to Azure SQL

-   Azure Function also calls textual analytics cognitive service to work out sentiment of tweet

-   Power BI imports data into it from Azure SQL and renders pre-defined reports
