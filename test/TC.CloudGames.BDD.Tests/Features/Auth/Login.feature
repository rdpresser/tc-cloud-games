Feature: User Authentication

  Ensures the login process behaves correctly under various conditions.

Scenario: Failed login attempt for admin with incorrect password
    Given the user enters the username "admin@admin.com"
    And the password "Wrong-Password-Admin-123"
    When they submit the login request
    Then the response should not include a JWT token

Scenario: Failed login attempt for regular user with incorrect password
    Given the user enters the username "user@user.com"
    And the password "Wrong-Password-User-123"
    When they submit the login request
    Then the response should not include a JWT token

Scenario: Attempted login with a non-existent username
    Given the user enters the username "fake@User.com"
    And the password "Fake-Password-123"
    When they submit the login request
    Then the response should not include a JWT token

Scenario: Login attempt with empty credentials
    Given the user leaves the username field ""
    And the password field ""
    When they submit the login request
    Then the response should not include a JWT token

Scenario: Successful login as administrator
    Given the user enters the username "admin@admin.com"
    And the password "Admin@123"
    When they submit the login request
    Then a JWT token should be generated in the response

Scenario: Successful login as standard user
    Given the user enters the username "user@user.com"
    And the password "User@123"
    When they submit the login request
    Then a JWT token should be generated in the response