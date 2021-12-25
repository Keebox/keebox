@E2E
Feature: Login
Exists a possibility to log into account using the provided token

    Scenario: Login with token
        Given an ordinary account and its token
        Given the valid account token
        When the login request has been sent
        Then the status code should be 200
        And access token should be returned
        And access token should be in cookie

    Scenario: Login with non existing token
        Given the wrong account token
        When the login request has been sent
        Then the status code should be 404
        And the message of error should be 'Account with this token does not exists.'
        And the status code in message should be 404