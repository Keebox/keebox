@E2E
Feature: RoleManagement
All operations with roles can be performed

    Scenario: Creating role with already existing name
        Given a new created role
        Given same role name in body
        And method is 'POST'
        And endpoint is '/role'
        When request has been sent
        Then the status code should be 409
        And response body is error

    Scenario: Getting roles via admin account
        Given an admin account and its token
        Given method is 'GET'
        And endpoint is '/role'
        When request has been sent
        Then the status code should be 200
        And roles should be returned

    Scenario: Getting roles via ordinary account
        Given an ordinary account and its token
        Given method is 'GET'
        And endpoint is '/role'
        When request has been sent
        Then the status code should be 403
        And response body is error

    Scenario: Getting specific role
        Given a new created role
        Given method is 'GET'
        And endpoint is '/role/{RoleId}'
        When request has been sent
        Then the status code should be 200
        And role should be returned

    Scenario: Getting non existing role
        Given a non existing role id
        Given method is 'GET'
        And endpoint is '/role/{RoleId}'
        When request has been sent
        Then the status code should be 404
        And response body is error

    Scenario: Getting specific role via ordinary account
        Given a new created role
        Given an ordinary account and its token
        Given method is 'GET'
        And endpoint is '/role/{RoleId}'
        When request has been sent
        Then the status code should be 403
        And response body is error

    Scenario: Updating role with new name
        Given a new created role
        Given a new role name
        Given method is 'PUT'
        And endpoint is '/role/{RoleId}'
        And role is in body
        When request has been sent
        Then the status code should be 204
        Given method is 'GET'
        When request has been sent
        Then the status code should be 200
        And role should be returned

    Scenario: Updating role via ordinary account
        Given a role id
        Given an ordinary account and its token
        Given method is 'PUT'
        And endpoint is '/role/{RoleId}'
        And role is in body
        When request has been sent
        Then the status code should be 403
        And response body is error

    Scenario: Deleting role by id
        Given a new created role
        Given method is 'DELETE'
        And endpoint is '/role/{RoleId}'
        When request has been sent
        Then the status code should be 204
        Given method is 'GET'
        When request has been sent
        Then the status code should be 404
        And response body is error

    Scenario: Deleting role via ordinary account
        Given a new created role
        Given an ordinary account and its token
        Given method is 'DELETE'
        And endpoint is '/role/{RoleId}'
        When request has been sent
        Then the status code should be 403
        And response body is error

    Scenario: Deleting non existing role
        Given a non existing role id
        Given method is 'DELETE'
        And endpoint is '/role/{RoleId}'
        When request has been sent
        Then the status code should be 404
        And response body is error