<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MessageDisplayer.ascx.cs" Inherits="CprBroker.Web.Pages.Controls.MessageDisplayer" %>

<script type="text/javascript">
    <!-- Script to show messages -->
    function openDialog(dialog) {
        var div = document.getElementById(dialog);
        if (div.style.display !== 'none') {
            div.style.display = 'none';
        }
        else {
            div.style.display = 'block';
        }
    };
</script>

<div id="dialog-box" style="display: none;">

    <div class="dialog-box-inner">
        <button runat="server" id="close1" onclick="openDialog('dialog-box')" class="close">Close</button>
        <textarea class="dialog-box-inner2" runat="server" enableviewstate="false" id="divMessages" readonly="readonly"></textarea>

        <button id="close2" onclick="openDialog('dialog-box')" class="close" runat="server">Close</button>
    </div>

</div>
