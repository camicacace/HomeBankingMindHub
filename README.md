# HomeBankingMindHub

## Descripción del Proyecto
HomeBanking es una aplicación de banco en línea diseñada para ofrecer una forma sencilla y segura de gestionar las finanzas personales. Los usuarios pueden controlar sus cuentas bancarias, tarjetas de crédito y débito, realizar transacciones y solicitar préstamos de manera rápida y sencilla.

![completado](https://img.shields.io/badge/estado-completado-brightgreen)

## ✅Tecnologías Utilizadas
  - **ASP.NET Core**: El marco principal para construir la API web.
  - **Entity Framework Core**: Un ORM (Object-Relational Mapper) que facilita la interacción con la base de datos, permitiendo un acceso y manejo de datos más intuitivo y eficiente.
  - **Microsoft SQL Server**: La base de datos relacional utilizada para almacenar toda la información del sistema.
  - **Identity**: Utilizado para la gestión de la autenticación y autorización de los usuarios. Incluye el uso de PasswordHasher<TUser> para el almacenamiento seguro de contraseñas mediante hashes criptográficamente seguros.
  - **Cookies**: Implementadas para manejar las sesiones de los usuarios.
  - **Swagger/OpenAPI**: Implementado para generar documentación interactiva de la API, facilitando el entendimiento y exploración de los endpoints disponibles.

## :hammer:Funcionalidades del Proyecto

- `Autenticación y Autorización`: Implementa un sistema robusto de autenticación y autorización para proteger la información del usuario.
- `Gestión de Cuentas Bancarias`: Permite a los usuarios visualizar y administrar sus cuentas bancarias.
- `Gestión de Tarjetas`: Permite crear nuevas tarjetas de crédito y débito y sus detalles.
- `Transacciones`: Facilita la realización de transacciones financieras tanto a cuentas propias como a cuentas de terceros.
- `Solicitud de Préstamos`: Permite a los usuarios solicitar préstamos de manera rápida y sencilla.
- `Documentación Interactiva`: Gracias a Swagger/OpenAPI, los desarrolladores pueden explorar y entender fácilmente los endpoints de la API.

## Notas
[1]: La rama "extra" contiene una funcionalidad adicional de manejo de tokens para la autenticacion.
