

//Trigger logic keydown 'enter' for form submission
const form = document.querySelector('form');
document.addEventListener('keypress', function (e) {
    if (e.key === 'Enter' && (e.target.tagName === 'INPUT' || e.target.tagName === 'TEXTAREA')) {
        e.preventDefault();
        return false;
    }
});

let debounceTimer;
let authorSelected = [];
document.getElementById('AuthorSearch').addEventListener('keyup', function () {
    const keyword = this.value;

    clearTimeout(debounceTimer);

    if (keyword === '') {
        const suggestionBox = document.getElementById('AuthorSuggestions');
        suggestionBox.style.display = 'none';
        renderSelectedAuthors();
        return;
    }

    debounceTimer = setTimeout(() => {
        searchAuthor(keyword);
    }, 500);

});


async function searchAuthor(keyword) {

    if (keyword.length < 2) {
        return;
    }

    const response = await fetch(`/Author/Search?name=${encodeURIComponent(keyword)}`);
    const authors = await response.json();
    console.log(authors);
    let html = "";

    if (authors.data.authors.length > 0) {
        authors.data.authors.forEach(a => {
            html += `
                <div class="list-group-item list-group-item-action author-item"
                data-id="${a.id}"
                data-name="${a.name}"
                data-description="${a.description}
                ">
                ${a.name} | ${a.description}
                </div>
            `;
        });
    }

    html += `
        <div class="list-group-item list-group-item-action text-primary"
        id="AddNewAuthor">
        + Add new author: "${keyword}"
        </div>
    `;

    const suggestionBox = document.getElementById('AuthorSuggestions');
    suggestionBox.innerHTML = html;
    suggestionBox.style.display = 'block';
}

document.addEventListener('DOMContentLoaded', () => {
  document.addEventListener('click', async function (e) {
      if ( e.target && e.target.id === 'AddNewAuthor') {
        console.log(e.target, "e target");
        const name = document.getElementById('AuthorSearch').value;
          if (!name) return;
            const response = await fetch('/Author/CreateAuthors', {
                  method: 'POST',
                  headers: {
                       'Content-Type': 'application/x-www-form-urlencoded'
                  },
                  body: `title=${encodeURIComponent(name)}`
            });
          const author = await response.json();
          console.log("authors", author);
          authorSelected.push({
              authorName: author.data.name,
          })
          renderSelectedAuthors();
      }
      if (e.target && e.target.classList.contains('author-item')) {
          const authorId = e.target.dataset.id;
          const authorName = e.target.dataset.name;
          const description = e.target.dataset.description.trim();
          if (!authorSelected.some(a => a.id === authorId)) {
              authorSelected.push({
                  authorId, authorName, description
              })
          }
          console.log(authorSelected, "authorSelected");
          renderSelectedAuthors();
      }
  });
});

function renderSelectedAuthors() {
    const authorIdInput = document.getElementById('AuthorIds');
    const authorSearch = document.getElementById('AuthorSearch');
    let html = "";
    authorSelected.forEach(author => {
        html += `<ul style="display: d-flex gap-3"
                    data-id="${author?.authorId}"
                    data-name="${author?.authorName}" >
                      <li> ${author?.authorName} | ${author?.description || ''}  </li> 
                    </ul>`
    });
    const suggestionBox = document.getElementById('AuthorSuggestions');
    suggestionBox.innerHTML = html;
    authorSearch.value = "";
    console.log("authorSelected ", authorSelected);
    authorIdInput.value = authorSelected.length ? JSON.stringify(authorSelected) : '';
    console.log("authorIdInput.value", authorIdInput.value);
    suggestionBox.style.display = authorSelected.length > 0 ? 'block' : 'none';
}


let currentStep = 1;


function showStep(step) {
    document.querySelectorAll(".step").forEach(s => s.classList.remove("active"));
    document.getElementById(`step-${step}`).classList.add("active");

    document.querySelectorAll(".step-item").forEach(s => s.classList.remove("active"));
    document.getElementById(`indicator-${step}`).classList.add("active");

}

function nextStep() {
    if (currentStep < 3) {
        const currentDiv = document.getElementById(`step-${currentStep}`);
        console.log("currentDivStep .... ", currentDiv);
        const inputs = currentDiv.querySelectorAll(".required-field")
        let costPrice = 0;

        for (let input of inputs) {
            console.log("Input ....", input);
            if (input.value.trim() === "") {
                alert("Please fill all fields before proceeding.");
                input.focus();
                return;
            }

            if (input.id === "costprice") {
                costPrice = parseFloat(input.value);
            }

            if (input.id === "saleprice") {
                const salePrice = parseFloat(input.value);
                if (salePrice < costPrice) {
                    alert("Price must be higher orginal price.");
                    input.focus();
                    return;
                }
                else if (salePrice < 0) {
                    alert("Price must be 0 or higher");
                    input.focus();
                    return;
                }
            }

            if (input.id === "inventory" && parseInt(input.value) < 0) {
                alert("Inventory must be 0 or higher.");
                input.focus();
                return;
            }   
        }
        currentStep++;
        console.log("currentStep ....", currentStep); 
        showStep(currentStep);
    }
}

function prevStep() {
    if (currentStep >= 1) {
        currentStep--;
        showStep(currentStep);
    }
}




