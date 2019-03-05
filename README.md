# SNAP - Employee On-boarding

New employees are expected to "hit the ground running" on their first day in a new organization. In order for this to happen, the organization must have all of the initial requirements prepared for their new employees ahead of time. That is why I developed an automated workflow around the employee on-boarding process.

The workflow initiates after HR saves the new employee data into their system. The data is then automatically pulled into a request queue, which is maintained in a custom application called SNAP. Each request branches into 3 main streams: automated tasks, manual task assignments, and notifications.

## Automated tasks
These are the tasks that the workflow is capable of completing without any manual intervention. This is possible due to the direct integration the workflow has with these services through API calls. The workflow takes the collected employee data and applies it to each service where applicable. Some examples of accounts that get automatically provisioned are:

* Active Directory for network access
* Microsoft Exchange for email
* Skype for Business for instant messaging and collaboration

The employee data is not only used to fill out properties for these accounts, but it also determines other settings such as:

* Adding to security and distribution groups based on department
* Assigning a phone number based on location
* Enabling certain application features based on role

## Manual Task Assignments
Not every task can be automatically completed. This may be due to a service not offering any APIs, there may be security restrictions, or some tasks just require physical work. Regardless, these tasks cannot be excluded from the process. This is where the second stream comes in.

The workflow leverages the organization's service desk system. It opens a service request which represents the overall new hire request. Then, as part of this service request, it will create sub tasks for each manual task that is required. Since the requirements for each employee are different, the workflow determines which sub tasks to open.

Not only is the content of each sub task filled out by the workflow, but with some added configuration, it determines who they should be assigned to. This way the task assignments go directly to the people responsible.

## Notifications
The on-boarding process involves effort from members of multiple teams. This primarily includes HR, IT and the hiring team. It is critical that information is clearly communicated between each team on time. That is why the third stream focuses on notifications. During key moments of the process, the workflow can send specific information to the appropriate recipients. This is one reason why updating members of distribution groups automatically is beneficial.

Some examples of these notifications are:

* Alerting teams when requests are submitted, updated or cancelled
* Providing new employee's initial login details to hiring manager
* Providing help and other information to new employee

## Automated Workflow Implementation
All of the scripts that determine the conditions and perform the actions of the workflow are written in PowerShell, a scripting language from Microsoft. The main reason for choosing PowerShell is because of how it's tightly coupled with most Microsoft services. Since I work for an organization that his a Microsoft centric infrastructure, it was the best fit.

## SNAP
To provide the workflow with a user interface and a way to maintain a SQL database, I developed a web application using the ASP.NET MVC framework. This gave me the ability to create an interface to plug or... SNAP... in my different PowerShell scripts. It also allowed me to provide dynamic web pages to display data which would be accessible to anyone that needed to track each request. It also provided a way for the different teams to add optional input.
