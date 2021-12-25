@E2E
Feature: RoleManagement
All operations with roles can be performed

    Scenario: Creating role with already existing name
        Given a new created role
        When a create role request with the same name has been sent
        Then the status code should be 409
        And the status code in message should be 409
        And the message of error should be 'Role already exists.'

    Scenario: Getting roles via admin account
        Given an admin account and its token
        When the get all roles request has been sent
        Then the status code should be 200
        And roles should be returned

    Scenario: Getting roles via ordinary account
        Given an ordinary account and its token
        When the get all roles request has been sent
        Then the status code should be 403
        And the message of error should be 'You don't have permission to perform this action.'

    Scenario: Getting specific role
        Given a new created role
        When a get role request by id has been sent
        Then the status code should be 200
        And role should be returned

    Scenario: Getting non existing role
        Given a non existing role id
        When a get role request by id has been sent
        Then the status code should be 404
        And the message of error should be 'Role not found.'
        And the status code in message should be 404

    Scenario: Getting specific role via ordinary account
        Given a new created role
        Given an ordinary account and its token
        When a get role request by id has been sent
        Then the status code should be 403
        And the message of error should be 'You don't have permission to perform this action.'

    Scenario: Updating role with new name
        Given a new created role
        Given a new role name
        When an update role request has been sent
        Then the status code should be 204
        When a get role request by id has been sent
        Then the status code should be 200
        And role should be returned

    Scenario: Updating role via ordinary account
        Given a role id
        Given an ordinary account and its token
        When an update role request has been sent
        Then the status code should be 403
        And the message of error should be 'You don't have permission to perform this action.'

    Scenario: Deleting role by id
        Given a new created role
        When a delete request has been sent
        Then the status code should be 204
        And role should not exists

    Scenario: Deleting role via ordinary account
        Given a new created role
        Given an ordinary account and its token
        When a delete request has been sent
        Then the status code should be 403
        And the message of error should be 'You don't have permission to perform this action.'

    Scenario: Deleting non existing role
        Given a non existing role id
        When a delete request has been sent
        Then the status code should be 404
        And the message of error should be 'Role not found.'
        And the status code in message should be 404