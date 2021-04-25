var loadedTags = [];
var createdTags = [];
var selectedTags = [];

var t_ul;
var st_ul;
var t_input;

function LoadTags(tags) {
    loadedTags = tags;
}

function SetInput(selector) {
    // = $("input[type=text]");
    t_input = $(selector);
}

function SetTagListForChoice(selector) {
    // = $("div.list-group#tList");
    t_ul = $(selector);
}

function SetTagListForSelected(selector) {
    // = $("#stList");
    st_ul = $(selector);
}

function Tune() {

    t_input.bind('cut copy paste', function (e) {
        e.preventDefault();
    });

    t_input.on("keydown", function (e) {
        if (e.which == 186) {
            e.stopPropagation();
            e.preventDefault();
            e.returnValue = false;
            e.cancelBubble = true;
            return false;
        }
        if (e.which == 13) {
            e.preventDefault();
        }
    });

    t_input.on("keyup", function (e) {
        var value = $(this).val().toLowerCase().trim();
        if (e.which == 13 && value != "") {
            ProcessTag(value);
            this.value = "";
            value = "";
        }
        if (value == "") {
            t_ul.hide();
        }
        else {
            t_ul.show();
            t_ul.children().filter(function () {
                var childName = this.innerText;
                $(this).toggle($(this).find("label#tName").text().toLowerCase().indexOf(value) > -1 && selectedTags.indexOf(childName) == -1);
            })
        }
    });

    for (var i = 0; i < loadedTags.length; i++)
        AppendTagToChoice(loadedTags[i]);

}

function AppendTagToChoice(name) {
    t_ul.append('<li class="list-group-item"><label id="tName" onclick="ChooseTag(\'' + name + '\')">' + name + '</label></li>');
}

function ProcessTag(name) {
    if (loadedTags.indexOf(name) == -1 && createdTags.indexOf(name) == -1) {
        createdTags.push(name);
        AppendTagToChoice(name);
    }
    if (selectedTags.indexOf(name) == -1) {
        selectedTags.push(name);
        AppendTagToSelected(name);
    }
}

function AppendTagToSelected(name) {
    st_ul.append("<span class='badge badge-secondary'>" + name + ";</span>");
    st_ul.append("<input name='tags.SelectedTags' value='"+name+"' hidden/>");
}

function ChooseTag(name) {
    t_input.val(name);
    t_input.focus();
}

function RemoveLastSelectedTag() {
    var name = selectedTags.pop();
    st_ul.find("span").filter(function () {
        if (this.innerHTML == name + ";") $(this).remove();
    });
    st_ul.find("input[name='tags.SelectedTags']").filter(function () {
        if (this.value == name) $(this).remove();
    });
}