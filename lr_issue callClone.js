/* *****************************************************************************
Revisions:
    Rev Date            Modified By         Description
***************************************************************************** */
var item = document.thisItem.getInnovator();
var res = item.applyMethod("lr_issue Clone", "<id>" + document.thisItem.getID() + "</id>");

if (res.isError()) {
    top.aras.AlertError(res.getErrorString());
} else {
    top.aras.uiShowItem("lr_issue", res.getAttribute("id"));
}
