
let debounceTimer;

document.getElementById('AuthorSearch').addEventListener('keyup', function () {
    const keyword = this.value;

    clearTimeout(debounceTimer);

    debounceTimer = setTimeout(() => {
        searchAuthor(keyword);
    }, 500);

});


async function searchAuthor(keyword) {

    if (keyword.length < 2) {
        return;
    }

    const response = await fetch(`/Author/Search?keyword=${encodeURIComponent(keyword)}`);
    const authors = await response.json();
    console.log(authors);
    let html = "";

    if (authors.length > 0) {
        authors.forEach(a => {
            html += `
                <div class="list-group-item list-group-item-action author-item"
                data-id="${a.id}"
                data-name="${a.name}">
                ${a.name}
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

        const response = await fetch('/Author/CreateAuthors', {
              method: 'POST',
              headers: {
                   'Content-Type': 'application/x-www-form-urlencoded'
              },
              body: `title=${encodeURIComponent(name)}`
        });
        const author = await response.json();
          const authorSearch = document.getElementById('AuthorSearch');
          authorSearch.value = author.data.name;
          let html = "";
          html = `<ul style="display: d-flex gap-3"
                data-id="${author.data.id}"
                data-name="${author.data.name}">
                  <li> ${author.data.name} </li> 
                </ul>`
          const suggestionBox = document.getElementById('AuthorSuggestions');
          suggestionBox.innerHTML = html;
          authorSearch.value = "";
          suggestionBox.style.display = 'block';
          //if (isSuccess == true) {
          //}
          //else {
          //    document.getElementById('AuthorSearch').value = 'Không thể thêm';
          //    document.getElementById('AuthorSuggestions').style.display = 'none';
          //}
     }
  });
});

