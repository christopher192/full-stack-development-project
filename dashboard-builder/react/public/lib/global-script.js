const defaultChartColorsGrapesJS = ["#3366CC", "#DC3912", "#FF9900", "#109618", "#990099", "#3B3EAC", "#0099C6", "#D47", "#6A0", "#B82E2E", "#316395", "#949", "#2A9", "#AA1", "#63C", "#E67300", "#8B0707", "#329262", "#5574A6", "#651067"];

function randomIntFromInterval(min, max) {
    return Math.floor(Math.random() * (max - min + 1) + min);
};

var intervalVar;
var widgetBoxList = [];

function setIntervalF() {
    console.log(typeof intervalVar === 'undefined');
    console.log(intervalVar);

    if (typeof intervalVar === 'undefined') {
        intervalVar = setInterval(function() {
            widgetBoxList.forEach(x => {
                let randomNbr = randomIntFromInterval(100, 500);
                let el = document.getElementById(x);
                el.innerHTML = randomNbr + " K";
            });
            console.log("I am invisible.");
        }, 3000);
    }
    else {
        clearInterval(intervalVar);

        intervalVar = setInterval(function() {
            widgetBoxList.forEach(x => {
                let randomNbr = randomIntFromInterval(100, 500);
                let el = document.getElementById(x);
                el.innerHTML = randomNbr + " K";
            });
            console.log("I am invisible.");
        }, 3000);
    }
}

function removeElement(element) {
    element && element.parentNode && element.parentNode.removeChild(element);
}

function OnSelectTable(selectedValue, mssqlTable) {
    let tablesColumns = mssqlTable;

    if (selectedValue !== "default") {
        if (tablesColumns[selectedValue].length > 0) {
            let parent = document.getElementById("showColumns");

            if (parent.children.length > 0) {
                let child = parent.firstElementChild;
                parent.removeChild(child);
            }

            let breakElement = document.createElement("br");

            let childFormGroup = document.createElement('div');
            childFormGroup.className = "form-group mb-3";

            let labelForSelectedColumns = document.createElement('label');
            labelForSelectedColumns.innerHTML = "Columns";
            labelForSelectedColumns.setAttribute("for", "selectedColumns");
    
            childFormGroup.appendChild(labelForSelectedColumns);
            childFormGroup.appendChild(breakElement);

            tablesColumns[selectedValue].map(val => { 
                let childInlineCheckBoxContainer = document.createElement('div');
                childInlineCheckBoxContainer.className = "form-check form-check-inline";

                let formCheckInput = document.createElement('input');
                formCheckInput.className = "form-check-input";
                formCheckInput.type = "checkbox";
                formCheckInput.name = "datatable-setting-column-input";
                formCheckInput.checked = true;
                formCheckInput.value =  val['colName'];
                formCheckInput.onchange = function (event) {
                    let id = val['colName'] + "-searchbox";
                    let checkb = document.getElementById(id);
                    
                    if (!this.checked) {
                        if (checkb.disabled === false) {
                            checkb.disabled = true;
                        }
                    } else {
                        if (checkb.disabled === true) {
                            checkb.disabled = false;
                        }
                    }
                };

                let formCheckLabel = document.createElement('label');
                formCheckLabel.className = "form-check-label";
                formCheckLabel.innerHTML = val['colName'];

                childInlineCheckBoxContainer.appendChild(formCheckInput);
                childInlineCheckBoxContainer.appendChild(formCheckLabel);

                let childInlineCheckBoxContainer2 = document.createElement('div');
                childInlineCheckBoxContainer2.className = "form-check form-check-inline";

                let formCheckInput2 = document.createElement('input');
                formCheckInput2.className = "form-check-input";
                formCheckInput2.type = "checkbox";
                formCheckInput2.name = "datatable-setting-searchbox-input";
                formCheckInput2.checked = true;
                formCheckInput2.value =  val['colName'];
                formCheckInput2.id = val['colName'] + "-searchbox";

                let formCheckLabel2 = document.createElement('label');
                formCheckLabel2.className = "form-check-label";
                formCheckLabel2.innerHTML = val['colName'] + " (Search Box)";

                childInlineCheckBoxContainer2.appendChild(formCheckInput2);
                childInlineCheckBoxContainer2.appendChild(formCheckLabel2);

                childFormGroup.appendChild(childInlineCheckBoxContainer);
                childFormGroup.appendChild(childInlineCheckBoxContainer2);

                let breakElement = document.createElement("br");
                childFormGroup.appendChild(breakElement);
            });

            parent.appendChild(childFormGroup);
        }
        
        const parent2 = document.getElementById('showColumns');
        const computedStyle = window.getComputedStyle(parent2);
        const display = computedStyle.getPropertyValue('display');

        if (display === 'none') {
            parent2.style.display = "block";
        }
    }
    else {
        let parent = document.getElementById("showColumns");

        if (parent.children.length > 0) {
            let child = parent.firstElementChild;
            parent.removeChild(child);
        }

        const parent2 = document.getElementById('showColumns');
        const computedStyle = window.getComputedStyle(parent2);
        const display = computedStyle.getPropertyValue('display');

        if (display === 'block') {
            parent2.style.display = "none";
        }
    }
}