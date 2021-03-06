﻿using System.Management.Automation;
using Okta.Core.Models;

namespace Okta.Core.Automation
{
    [Cmdlet(VerbsCommon.Remove, "OktaGroupMember")]
    public class RemoveOktaGroupMember : OktaCmdlet
    {
        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            Position = 0,
            HelpMessage = "Id or name of the group to retrieve"
        )]
        public string IdOrName { get; set; }

        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            Position = 1,
            HelpMessage = "Id or user name (login) of the user to remove from the group"
        )]
        public string UserIdOrLogin { get; set; }

        protected override void ProcessRecord()
        {
            var groupsClient = Client.GetGroupsClient();
            Models.Group group = null;
            Models.User user = null;
            try
            {
                group = groupsClient.Get(IdOrName);
            }
            catch (OktaException)
            {
                group = groupsClient.GetByName(IdOrName);
            }
            if (group != null)
            {
                var groupUsersClient = Client.GetGroupUsersClient(group);
                var usersClient = Client.GetUsersClient();

                try
                {
                    user = usersClient.Get(UserIdOrLogin);
                }
                catch (OktaException)
                {
                    WriteWarning(string.Format("The user with ID or name {0} is invalid. Please try again.", UserIdOrLogin));
                }

                if (user != null)
                {
                    groupUsersClient.Remove(user);
                    WriteObject(string.Format("User {0} was successfully removed from group {1}", user.Profile.Login, group.Profile.Name));
                }
            }
            else
            {
                WriteWarning(string.Format("The group with id or name {0} seems to be invalid, please try with a different value", IdOrName));
            }
        }
    }
}
