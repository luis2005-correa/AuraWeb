#  AuraWeb - Plataforma de Comercio Electrónico

> **Proyecto Real:** Página de venta de ropa creada para un cliente, la página contiene autenticación, gestión de productos, carrito de compras, panel administrativo y estadístico.

---

##  Descripción

**AuraWeb** es un sistema web full-stack diseñado para la venta e interacción comercial de productos de moda. Integra una tienda pública intuitiva para los clientes y un panel de administración avanzado para la gestión de inventario, usuarios y análisis estadístico del negocio.

---

##  Características Principales

###  **Experiencia del Cliente (Storefront)**
* **Autenticación Segura:** Registro e inicio de sesión integrados con Firebase Authentication.
* **Catálogo interactivo:** Navegación, filtrado y vista detallada de productos/ropa.
* **Carrito de compras:** Gestión dinámica de artículos, cálculo de totales y preparación para Checkout.

### **Panel Administrativo (Backoffice)**
* **Gestión de Productos (CRUD):** Alta, baja, edición y control de stock de prendas.
* **Panel Estadístico:** Gráficas e indicadores clave sobre ventas, productos más populares y métricas del comercio.
* **Control de Accesos:** Roles diferenciados para clientes y administradores.

---

##  Stack Tecnológico

* **Backend:** C# / .NET Core (ASP.NET Core MVC)
* **Frontend:** Razor Views, JavaScript, HTML5, Tailwind CSS
* **Base de Datos:** PostgreSQL
* **Autenticación:** Firebase Authentication
* **Despliegue / Contenedores:** Docker

---

```bash
docker build -t aura-web .
docker run -p 8080:80 aura-web
