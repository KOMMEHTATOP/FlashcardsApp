(function () {
    window.dragDropInterop = {
        initializeDragDrop: function (authToken) {
            console.log("=== dragdrop: init ===");
            console.log("Received token:", authToken ? "YES" : "NO");

            const container = document.getElementById('groups-container');
            if (!container) {
                console.log("dragdrop: groups-container не найден");
                return;
            }

            const cards = container.querySelectorAll('.group-card');
            console.log(`dragdrop: найдено карточек = ${cards.length}`);
            if (!cards.length) return;

            let dragged = null;

            cards.forEach(function (card, idx) {
                card.draggable = true;
                card.style.cursor = 'grab';
                console.log(`dragdrop: init card[${idx}] id=${card.dataset.groupId}`);

                card.addEventListener('dragstart', function (e) {
                    dragged = this;
                    e.dataTransfer.effectAllowed = 'move';
                    try { e.dataTransfer.setData('text/plain', this.dataset.groupId); } catch {}
                    this.style.opacity = '0.5';
                });

                card.addEventListener('dragend', function () {
                    this.style.opacity = '1';
                    dragged = null;
                });

                card.addEventListener('dragover', function (e) {
                    e.preventDefault();
                    this.style.border = '2px solid #0d6efd';
                });

                card.addEventListener('dragleave', function () {
                    this.style.border = '';
                });

                card.addEventListener('drop', function (e) {
                    e.preventDefault();
                    this.style.border = '';
                    if (!dragged || dragged === this) return;

                    const draggedCol = dragged.closest('.col-md-4, .col');
                    const targetCol = this.closest('.col-md-4, .col');
                    if (!draggedCol || !targetCol) return;

                    // Определяем позиции элементов
                    const draggedIndex = Array.from(container.children).indexOf(draggedCol);
                    const targetIndex = Array.from(container.children).indexOf(targetCol);

                    // Вставляем в зависимости от направления
                    if (draggedIndex < targetIndex) {
                        // Перетаскиваем вниз/вправо - вставляем ПОСЛЕ целевого
                        targetCol.parentNode.insertBefore(draggedCol, targetCol.nextSibling);
                    } else {
                        // Перетаскиваем вверх/влево - вставляем ПЕРЕД целевым
                        targetCol.parentNode.insertBefore(draggedCol, targetCol);
                    }
                    const data = [];
                    let order = 1;
                    container.querySelectorAll('.group-card').forEach(function (c) {
                        data.push({ Id: c.dataset.groupId, Order: order++ });
                    });

                    fetch('http://localhost:5153/api/group/reorder', {
                        method: 'PUT',
                        headers: {
                            'Content-Type': 'application/json',
                            'Authorization': 'Bearer ' + authToken
                        },
                        body: JSON.stringify(data)
                    }).then(r => {
                        if (!r.ok) alert('Ошибка изменения порядка');
                    });
                });
            });

            // корзина
            const trash = document.querySelector('.trash-zone');
            if (trash) {
                trash.addEventListener('dragover', e => {
                    e.preventDefault();
                    trash.style.backgroundColor = '#f8d7da';
                });
                trash.addEventListener('dragleave', () => trash.style.backgroundColor = '');
                trash.addEventListener('drop', e => {
                    e.preventDefault();
                    trash.style.backgroundColor = '';
                    let id = null;
                    try { id = e.dataTransfer.getData('text/plain'); } catch {}
                    if (!id) return;
                    if (!confirm('Удалить группу?')) return;

                    fetch('/api/group/' + id, {
                        method: 'DELETE',
                        headers: { 'Authorization': 'Bearer ' + authToken }
                    }).then(r => {
                        if (r.ok) location.reload();
                        else alert('Ошибка удаления группы');
                    });
                });
            }

            console.log("=== dragdrop: ready ===");
        }
    };
})();
