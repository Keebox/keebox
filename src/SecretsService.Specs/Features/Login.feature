@E2E
Feature: Login
Exists a possibility to log into account using the provided token

    Scenario: Login with token
        Given an ordinary account and its token
        Given private token in the body
        And method is 'POST'
        And endpoint is '/login'
        When request has been sent
        Then the status code should be 200
        And access token should be returned
        And access token should be in cookie

    Scenario: Login with non existing token
        Given the wrong account token
        Given account token set to request body
        Given method is 'POST'
        Given endpoint is '/login'
        When request has been sent
        Then the status code should be 404
        And response body is error