"# Dairymaster_Code_Challenge_CSharp" 

Endpoints include:\
getBooks - GET, Shows a list of books.\
createbook - POST, Creates a book and sets the status.\
publishbook - PATCH, Publishes a selected book and sets the date.\
listbooks - GET, List all books with publication status.

Technical: The project uses Scalar and is configured to this port: http://localhost:8000/scalar/#tag/bookwebapi

Why I chose my design:\
My main thinking process was to link the work of the project to the criteria of what I believe I would be dealing with in the company, so this is why I chose a minimal API approach. Presumably, minimal APIs are better suited for equipment which are remote with little connectivity and that run locally i.e. Farming software. I anticipate a lot of work with microservices which is also why I chose this method as they are favoured.

Project isn't very complex, so a minimal API is adequate. Whilst I have done controller-based projects before, I believe minimal APIs are better for systems with a lot of constraints and resource limits.

In a real-world environment, the program would be a lot more robust with other data and functionality, but I kept it inline with the scope of the document as much as possible.

Review:
I believe I was pretty close to setting up the final solution with SQLite until some elements of my code broke near the end. The code is still there, but it is commented out so the the project can function. This made me revert back to my earlier, more basic code. It works as intended, fullfilling the steps of the challenge, albeit with some rudimentary persistent data.