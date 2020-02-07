Feature: retrieve all loan transfer for an account holder
	As a User
	I want to retrieve all loan transfer details for a given account holder
	In order to use the loan transfer details

# vocabulary
# User : registered user with the bank to avail banking services
# ConsumerID : a unique ID that identifies an account holder

@positive
Scenario Outline: retrieve all loan transfer for a valid account holder
	Given the User is registered with the bank
	When the User provide valid <ConsumerID>
	Then all loan transfer details are  provided to the User
Examples:
	| ConsumerID |
	|  831       | 

@negative
Scenario Outline: retrieve all loan transfer for a invalid account holder
	Given the User is registered with the bank
	When the User provide an invalid <ConsumerID>
	Then there is no loan transfer details available 
	And 'Invalid consumer Id' message is provided to the user
Examples:
	| ConsumerID |
	|  111       | 
	
