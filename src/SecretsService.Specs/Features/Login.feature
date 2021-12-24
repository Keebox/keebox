@E2E
Feature: Login
Can get access token via account secret token provisioning

    Background:
        Given a temporary account

    Scenario: Login with token
        Given the account token
        When the login request has been sent
        Then the status code should be 200
        And access token should be returned
        And access token should be in cookie