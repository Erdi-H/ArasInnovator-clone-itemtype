/******************************************************************************
Revisions:
    Rev Date        Modified By      Description
*******************************************************************************/
//System.Diagnostics.Debugger.Break(); //enable/disable the debugger as required
string issueId = this.getProperty("id");
Innovator inn = this.getInnovator();

// Create AML query. Use innovator admin tool to verify your query.
// https://github.com/erdomke/InnovatorAdmin
String queryTemplate = @"<AML>
  <Item type='lr_issue' action='get' id='{0}'>
    <Relationships>
      <Item type='lr_Issue File' action='get'>
      </Item>
      <Item type='lr_issue_action_item' action='get'>
      </Item>
      <Item type='lr_issue_document' action='get'>
      </Item>
      <Item type='lr_issue_part' action='get'>
      </Item>
      <Item type='lr_issue_program' action='get'>
      </Item>
      <Item type='lr_issue_signoffs' action='get'>
      </Item>
    </Relationships>
  </Item>
</AML>";
 
// give permissions
Aras.Server.Security.Identity plmIdentity = Aras.Server.Security.Identity.GetByName("Aras PLM");
bool PermissionWasSet = Aras.Server.Security.Permissions.GrantIdentity(plmIdentity);

try {
    Item issueItem = inn.applyAML(String.Format(CultureInfo.InvariantCulture, queryTemplate, issueId));
    if (issueItem.isError()) {
        return issueItem;
    }
    Item clonedIssue = this.newItem("lr_issue", "add");

    // It may be easier to use getProperties() depending on how many you need
    // to copy over vs. not copy over.
    clonedIssue.setProperty("_currency", issueItem.getProperty("_currency", ""));
    clonedIssue.setProperty("_customer", issueItem.getProperty("_customer", ""));
    clonedIssue.setProperty("_estimated_opportunity", issueItem.getProperty("_estimated_opportunity", ""));
    clonedIssue.setProperty("_feasibility", issueItem.getProperty("_feasibility", ""));
    clonedIssue.setProperty("_plant_affected", issueItem.getProperty("_plant_affected", ""));
    clonedIssue.setProperty("_region", issueItem.getProperty("_region", ""));
    clonedIssue.setProperty("_target_impl_date", issueItem.getProperty("_target_impl_date", ""));
    clonedIssue.setProperty("_workshop_identifier", issueItem.getProperty("_workshop_identifier", ""));
    clonedIssue.setProperty("harness_fam", issueItem.getProperty("harness_fam", ""));
    clonedIssue.setProperty("issue_description", issueItem.getProperty("issue_description", ""));
    clonedIssue.setProperty("issue_severity", issueItem.getProperty("issue_severity", ""));
    clonedIssue.setProperty("issue_source", issueItem.getProperty("issue_source", ""));
    clonedIssue.setProperty("issue_title", issueItem.getProperty("issue_title", ""));
    clonedIssue.setProperty("issue_type", issueItem.getProperty("issue_type", ""));
    clonedIssue.setProperty("model_year", issueItem.getProperty("model_year", ""));
    clonedIssue.setProperty("owned_by_id", issueItem.getProperty("owned_by_id", ""));
    clonedIssue.setProperty("program_phase", issueItem.getProperty("program_phase", ""));
    clonedIssue.setProperty("reference_no", issueItem.getProperty("reference_no", ""));
    clonedIssue.setProperty("state", issueItem.getProperty("state", ""));
    clonedIssue.setProperty("target_date", issueItem.getProperty("target_date", ""));

    // Again, depending on how many relationships you want to copy vs. not copy
    // you might want to loop through an array of relationship names to reduce duplication
    Item issueRel = issueItem.getItemsByXPath("./Relationships/Item[@type='lr_Issue File']");
    for (int i = 0; i < issueRel.getItemCount(); ++i) {
        Item currentItem = issueRel.getItemByIndex(i);
        Item newItem = this.newItem("lr_Issue File", "add");
        newItem.setProperty("related_id", currentItem.getProperty("related_id", ""));
        clonedIssue.addRelationship(newItem);
    }
    issueRel = issueItem.getItemsByXPath("./Relationships/Item[@type='lr_issue_document']");
    for (int i = 0; i < issueRel.getItemCount(); ++i) {
        Item currentItem = issueRel.getItemByIndex(i);
        Item newItem = this.newItem("lr_issue_document", "add");
        newItem.setProperty("related_id", currentItem.getProperty("related_id", ""));
        clonedIssue.addRelationship(newItem);
    }
    issueRel = issueItem.getItemsByXPath("./Relationships/Item[@type='lr_issue_part']");
    for (int i = 0; i < issueRel.getItemCount(); ++i) {
        Item currentItem = issueRel.getItemByIndex(i);
        Item newItem = this.newItem("lr_issue_part", "add");
        newItem.setProperty("related_id", currentItem.getProperty("related_id", ""));
        clonedIssue.addRelationship(newItem);
    }
    issueRel = issueItem.getItemsByXPath("./Relationships/Item[@type='lr_issue_program']");
    for (int i = 0; i < issueRel.getItemCount(); ++i) {
        Item currentItem = issueRel.getItemByIndex(i);
        Item newItem = this.newItem("lr_issue_program", "add");
        newItem.setProperty("related_id", currentItem.getProperty("related_id", ""));
        clonedIssue.addRelationship(newItem);
    }

    Item result = clonedIssue.apply();
    if (result.isError()) {
        return inn.newError(result.getErrorDetail());
    }
    return result;
}
finally {
    // Revoke 'Aras PLM' permissions
    if (PermissionWasSet) Aras.Server.Security.Permissions.RevokeIdentity(plmIdentity);
}
