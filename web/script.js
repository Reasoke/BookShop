// const API_URL = 'http://localhost:7710/api/books';
const API_URL = 'https://molly-busy-jolly.ngrok-free.app/api/books';

function createRow(book) {
  const authors = book.authors.map(author => author.name).join(', ');
  const genres = book.genres.map(genre => genre.name).join(', ');

  return `
        <tr>
            <td>${book.id}</td>
            <td>${book.title}</td>
            <td>${book.price} грн</td>
            <td>${book.publisher}</td>
            <td>${book.description}</td>
            <td>${authors}</td>
            <td>${genres}</td>
        </tr>
    `;
}

async function loadBooks() {
  try {
    const response = await fetch(API_URL, {
      headers: {
        "ngrok-skip-browser-warning": "true",
      }
    });

    if (!response.ok) {
      throw new Error(`Ошибка сервера: ${response.status} ${response.statusText}`);
    }

    const books = await response.json();
    const tableBody = document.querySelector('#booksTable tbody');
    tableBody.innerHTML = books.map(createRow).join('');
  } catch (error) {
    console.error('Ошибка при загрузке книг:', error);

    document.querySelector('#booksTable tbody').innerHTML = `
            <tr>
                <td colspan="7">Ошибка загрузки данных: ${error.message}. Проверьте соединение или попробуйте позже.</td>
            </tr>`;
  }
}
document.addEventListener('DOMContentLoaded', loadBooks);

// const UPDATE_API_URL = 'http://localhost:7710/api/books/updateDesc';
const UPDATE_API_URL = 'https://molly-busy-jolly.ngrok-free.app/api/books/updateDesc';
// const AUTHORS_COUNT_API_URL = 'http://localhost:7710/api/books/authorsCountByRegion';
const AUTHORS_COUNT_API_URL = 'https://molly-busy-jolly.ngrok-free.app/api/books/authorsCountByRegion';
// const SALES_COUNT_API_URL = 'http://localhost:7710/api/books/everySecondBookBySalesCount';
const SALES_COUNT_API_URL = 'https://molly-busy-jolly.ngrok-free.app/api/books/everySecondBookBySalesCount';


document.addEventListener('DOMContentLoaded', () => {
  const updateButton = document.getElementById('updateButton');
  const genreInput = document.getElementById('genreInput');
  const updateResult = document.getElementById('updateResult');

  const authorsCountButton = document.getElementById('authorsCountButton');
  const regionInput = document.getElementById('regionInput');
  const authorsCountResult = document.getElementById('authorsCountResult');

  const salesCountButton = document.getElementById('salesCountButton');
  const salesInput = document.getElementById('salesInput');
  const salesBooksTable = document.getElementById('salesBooksTable').getElementsByTagName('tbody')[0];

  const addAuthorButton = document.getElementById('addAuthorButton');
  const bookInput = document.getElementById('bookInput');
  const authorIdInput = document.getElementById('authorIdInput');
  const addAuthorResult = document.getElementById('addAuthorResult');


  updateButton.addEventListener('click', async () => {
    const genreName = genreInput.value.trim();

    if (!genreName) {
      updateResult.textContent = 'Введіть жанр!';
      return;
    }

    try {
      const url = `${UPDATE_API_URL}?genreName=${encodeURIComponent(genreName)}`;

      const response = await fetch(url, {
        headers: {
          "ngrok-skip-browser-warning": "true",
        },
        method: 'POST',
      });

      if (!response.ok) {
        throw new Error(`Ошибка сервера: ${response.status} ${response.statusText}`);
      }

      const result = await response.json();

      updateResult.textContent = `Було оновлено стільки книг: ${result.booksUpdated}`;
    } catch (error) {
      console.error('Ошибка при обновлении описания:', error);
      updateResult.textContent = 'Ошибка обновления. Попробуйте позже.';
    }
  });

  authorsCountButton.addEventListener('click', async () => {
    const regionName = regionInput.value.trim();

    if (!regionName) {
      authorsCountResult.textContent = 'Введіть регіон!';
      return;
    }

    try {
      const url = `${AUTHORS_COUNT_API_URL}?region=${encodeURIComponent(regionName)}`;
      const response = await fetch(url, {
        headers: {
          "ngrok-skip-browser-warning": "true",
        }
      });

      if (!response.ok) {
        throw new Error(`Ошибка сервера: ${response.status} ${response.statusText}`);
      }

      const result = await response.json();
      authorsCountResult.textContent = `У регіоні ${regionName} було найдено стільки авторів: ${result.authorsCountByRegion}`;
    } catch (error) {
      console.error('Ошибка при подсчете авторов:', error);
      authorsCountResult.textContent = 'Ошибка подсчета. Попробуйте позже.';
    }
  });

  salesCountButton.addEventListener('click', async () => {
    const salesValue = salesInput.value.trim();

    if (!salesValue) {
      salesBooksTable.innerHTML = '<tr><td colspan="3">Введите количество продаж!</td></tr>';
      return;
    }

    try {
      const url = `${SALES_COUNT_API_URL}?value=${encodeURIComponent(salesValue)}`;
      const response = await fetch(url, {
        headers: {
          "ngrok-skip-browser-warning": "true",
        }
      });

      if (!response.ok) {
        throw new Error(`Ошибка сервера: ${response.status} ${response.statusText}`);
      }

      const result = await response.json();

      salesBooksTable.innerHTML = '';

      result.forEach(book => {
        const row = salesBooksTable.insertRow();
        row.insertCell(0).textContent = book.name;
        row.insertCell(1).textContent = book.salE_QUANTITY;
        row.insertCell(2).textContent = book.publisher;
      });
    } catch (error) {
      console.error('Ошибка при получении книг:', error);
      salesBooksTable.innerHTML = '<tr><td colspan="3">Ошибка получения данных. Попробуйте позже.</td></tr>';
    }
  });

  addAuthorButton.addEventListener('click', async () => {
    const bookName = bookInput.value.trim();
    const authorId = authorIdInput.value.trim();

    if (!bookName || !authorId) {
      addAuthorResult.textContent = 'Введіть ID книги та ID автора!';
      return;
    }

    try {
      const url = `${API_URL}/${encodeURIComponent(bookName)}/addAuthor?authorId=${encodeURIComponent(authorId)}`;
      const response = await fetch(url, {
        headers: {
          "ngrok-skip-browser-warning": "true",
        },
        method: 'POST',
      });

      const responseText = await response.text();
      try {
        const jsonResponse = JSON.parse(responseText);
        addAuthorResult.textContent = `Відповідь сервера: ${JSON.stringify(jsonResponse)}`;
      } catch {
        addAuthorResult.textContent = `Відповідь сервера (не JSON): ${responseText}`;
      }
    } catch (error) {
      addAuthorResult.textContent = `Ошибка при запросе: ${error.message}`;
    }
  });
});