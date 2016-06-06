

Array.prototype.remove = function (from, to) {
    var rest = this.slice((to || from) + 1 || this.length);
    this.length = from < 0 ? this.length + from : from;
    return this.push.apply(this, rest);
};
// name_chip moustache template service
function LoadNameChip(nameParam) {
    var template = $('#add_new_name_chip_template').html();
    Mustache.parse(template);   // optional, speeds up future uses
    var rendered = Mustache.render(template, { name: nameParam });
    $('#add_new_name_chip_container').append(rendered);
}

function LoadPreferenceInput(personName, orderNumber, nameList) {
    var template = $('#add_preferences_input_template').html();
    Mustache.parse(template);   // optional, speeds up future uses
    var rendered = Mustache.render(template, { person_name: personName, order_number: orderNumber, name_list: nameList });
    $('#add_preferences_input_container').append(rendered);

    // refresh materialize select style
    $('select').material_select();
}
function LoadAllPreferenceInputs(nameList) {
    $("#add_preferences_input_container").html("")
    for (index = 0; index < nameList.length; ++index) {
        var tempNameArray = nameList.slice();// copy only array values, not reference
        tempNameArray.remove(index)
        LoadPreferenceInput(nameList[index], index, tempNameArray);
        console.log("try add preference for: " + nameList[index]);
    }
}

// input field validation - based on Array:nameList name container
function ValidateAddNewNameInput(nameList) {
    var input = $('#add_new_name_input');

    if (input.val().trim() == "") {
        $('label[for=add_new_name_input]').attr('data-error', "New name can't be empty string.");
        input.addClass('invalid');
        return false;
    } else if (nameList.indexOf(input.val().trim()) != -1) {
        $('label[for=add_new_name_input]').attr('data-error', "Name \"" + input.val().trim() + "\" was added previously.");
        input.addClass('invalid');
        return false;
    } else {
        input.removeClass('invalid');
        return true;
    }
}

//==========   ~/Views/Home/Partial/_AddName.cshtml   ==========
$(document).ready(function () {

    // MATERIALIZE.CSS INITIALIZATIONS
    $('select').material_select();

    var nameList = new Array();

    // #add_new_name_button on click action
    $(document).on('click', '#add_new_name_button', function (e) {
        
        var input = $('#add_new_name_input');

        if (ValidateAddNewNameInput(nameList)) {
            nameList.push(input.val().trim());
            LoadNameChip(input.val().trim());
            input.val('');
        }

        input.focus();
    });
    $(document).on('keydown', '#add_new_name_input', function (e) {
        if (e.keyCode == 13) {
            e.preventDefault();
            $('#add_new_name_button').trigger('click');
        }
    });


    // name_chip on click/close action - delete related position in array
    $(document).on('click.chip', '#add_new_name_chip_container .chip .material-icons', function (e) {
        var nameToDelete = $(this).parent().attr('id').replace('_chip', '');

        var nameIndex = nameList.indexOf(nameToDelete);
        if (nameIndex > -1) {
            nameList.splice(nameIndex, 1);
        }

        $(this).parent().remove();
    });

    $(document).on('click', "#reload_preferences_input_button", function (e) {
        LoadPreloader('#add_preferences_input_container');
        LoadAllPreferenceInputs(nameList);
    });

//================== AJAX RESPONSE ==================
    $(document).on('click', '#submit_button', function (e) {
        e.preventDefault();
        if (nameList.length >= 3) {
            var people = $("#main_form").serialize();
            //var people2 = JSON.stringify($("#main_form"));
            //console.log(people);
            //console.log(people2);
            LoadPreloader('#result_container');

            $.ajax({
                url: '/Home/Index',//'@Url.Action("Index", "Home")',
                //contentType: 'application/html; charset=utf-8',
                contentType: 'application/x-www-form-urlencoded; charset=utf-8',
                data: people,
                type: 'Post',
                //dataType: 'json'
            })
            .success(function (result) {

                //var tempNameArray = new Array();
                if (result != undefined) {
                    var tempNameArray = $.map(result, function (el) { return el });
                    console.log(tempNameArray);
                    LoadResultList(tempNameArray);
                } else {
                    LoadResultMessage("Unsatisfable.");
                }
                console.log("after parsing ajax result");
                
            })
            .error(function (xhr, status) {
                console.log("error!!!");
                alert(status);//watch this
            })
        } else {
            alert("list of names is shorter than 3");
        }
    });

//==========   ~/Views/Home/Partial/_ShowResult.cshtml   ==========

    function LoadPreloader(containerID) {
        var template = $('#preloader_template').html();
        Mustache.parse(template);   // optional, speeds up future uses
        var rendered = Mustache.render(template);
        $(containerID).html(rendered);
    }

    function LoadResultList(resultList) {
        var template = $('#result_list_template').html();
        Mustache.parse(template);   // optional, speeds up future uses
        var rendered = Mustache.render(template, { result_list: resultList });

        $('#result_container').html(rendered);
    }

    function LoadResultMessage(message) {
        var template = $('#result_message_template').html();
        Mustache.parse(template);   // optional, speeds up future uses
        var rendered = Mustache.render(template, { message: message });

        $('#result_container').html(rendered);
    }
});




