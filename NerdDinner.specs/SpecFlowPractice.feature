Feature: SpecFlowPractice
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers


Background: 
	Given I have teeth
	And I have some toothpaste

@testGroup1
Scenario: Scenario A
	Given I'm using "<brand>" brand toothpaste
	When Ibrush for <mins> minutes
	Then the teeth look <percent> white

	Examples: 
		| brand   | mins | percent |
		| Brand X | 1    | 80      |
		| Brand Y | 3    | 100     |
		| Brand Z | 10   | 10      |
				
@testGroup2
Scenario: Scenario B
	Given I have entered 50 into the calculator
	And I have entered 70 into the calculator
	When I press add
	#When I press add
	Then the result should be 120 on the screen

@testGroup1
Scenario: Scenario C
	Given I have entered 50 into the calculator
	And I have entered 70 into the calculator
	When I press add
	Then the result should be 120 on the screen