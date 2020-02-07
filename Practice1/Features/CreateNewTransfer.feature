Feature: create new transfer between account holder loan accounts 
	As a User
	I want to create a new transfer between account holder's loan accounts
	In order transfer funds

# vocabulary
# User : registered user with the bank to avail banking services
# ConsumerID : a unique ID that identifies an account holder
# FromAccountID : Defines a unique ID that identifies the account where funds are transferred from
# ToAccountID : Defines the unique account ID for the account funds are transferred to
# StartDate : Defines the start date of the transfer. Default:  2018-03-25
#
@positive
Scenario Outline: transfer funds between loan accounts for a valid account holder by passing all required parameters
	Given the User is registered with the bank
	When the User provide a valid <ConsumerID>, <FromAccountID>, <ToAccountID>, <amount>, <StartDate>
	Then funds are transfered between account holder's loan accounts
	And loan transfer id is provided to the User
Examples:
	| ConsumerID | FromAccountID | ToAccountID | amount | StartDate   |
	| 831        | 10020         | 10023       | 1.00   | "2020-02-06"|

@negative
Scenario Outline: transfer funds between loan accounts for a valid account holder by passing some wrong information
	Given the User is registered with the bank
	When the User provide a valid <ConsumerID>, <FromAccountID>, <ToAccountID>, <amount>, <StartDate>
	Then funds are not transfered between account holder's loan accounts
    Then a message of invalid parameter is provided to the User
Examples:
	| ConsumerID | FromAccountID | ToAccountID | amount | StartDate   |
	| 831        |		11		 | 10023       | 3.00   | "2020-02-04"|