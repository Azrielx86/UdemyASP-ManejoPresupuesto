function initForm(urlGetCategories) {
    $("#TipoOperacionId").change(async function () {
        const valorSeleccionado = $(this).val();

        const respuesta = await fetch(urlGetCategories, {
            method: "POST",
            body: valorSeleccionado,
            headers: {
                "Content-Type": "application/json",
            },
        });

        const json = await respuesta.json();
        const opciones = json.map(categoria => `<option value=${categoria.value}>${categoria.text}</option>`);
        $("#CategoriaId").html(opciones);
    });
}