@E2E
Feature: AccountManagement
All operations with accounts can be performed

    Scenario: Getting all accounts
        Given an admin account and its token
        Given method is 'GET'
        And endpoint is '/account'
        When request has been sent
        Then the status code should be 200
        And accounts should be returned

    Scenario: Getting all accounts via non-admin account
        Given an ordinary account and its token
        Given method is 'GET'
        And endpoint is '/account'
        When request has been sent
        Then the status code should be 403
        And response body is error

    Scenario: Getting specific account
        Given an admin account and its token
        And an ordinary account and its token
        And an admin account and its token
        And id of existing account
        Given method is 'GET'
        And endpoint is '/account/{AccountId}'
        When request has been sent
        Then the status code should be 200
        And account should be returned

    Scenario: Getting specific account via non-admin account
        Given an admin account and its token
        And id of existing account
        And an ordinary account and its token
        Given method is 'GET'
        And endpoint is '/account/{AccountId}'
        When request has been sent
        Then the status code should be 403
        And response body is error

    Scenario: Creating account with generated token
        Given an admin account and its token
        Given method is 'POST'
        And endpoint is '/account'
        And with api generated token
        And type and name is in the body
        When request has been sent
        Then the status code should be 201
        And private token should be returned

    Scenario: Creating account with pregenerated token
        Given an admin account and its token
        Given method is 'POST'
        And endpoint is '/account'
        And with pregenerated api token
        And type and name is in the body
        When request has been sent
        Then the status code should be 201
        And private token should be like pregenerated

    Scenario: Creating account via non-admin account
        Given an admin account and its token
        And an ordinary account and its token
        Given method is 'POST'
        And endpoint is '/account'
        And with pregenerated api token
        And type and name is in the body
        When request has been sent
        Then the status code should be 403
        And response body is error

    Scenario: Deleting account
        Given an admin account and its token
        And an ordinary account and its token
        And an admin account and its token
        And id of existing account
        Given method is 'DELETE'
        And endpoint is '/account/{AccountId}'
        When request has been sent
        Then the status code should be 204
        Given method is 'GET'
        And endpoint is '/account/{AccountId}'
        When request has been sent
        Then the status code should be 404
        And response body is error

    Scenario: Deleting account via non-admin account
        Given an admin account and its token
        And id of existing account
        And an ordinary account and its token
        Given method is 'DELETE'
        And endpoint is '/account/{AccountId}'
        When request has been sent
        Then the status code should be 403
        And response body is error