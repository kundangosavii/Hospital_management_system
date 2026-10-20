## TASK A: Hospital Appointment Management System

Problem statement: Develop an ASP.NET Core MVC application to display doctor
information and manage patient appointment requests.
1. Model and Extension Method
o Create a Doctor model with DoctorId, DoctorName, Specialization,
Experience and ConsultationFee.
o Implement CalculateTotalCharge() to add a ₹100 service charge.
o Display doctor details and total consultation charges.
2. Asynchronous Web Request
o Retrieve posts asynchronously from
https://jsonplaceholder.typicode.com/posts.
o Display post ID and title in a table.
3. State Management
o Cookie: Store patient name.
o Session: Store selected specialization.
o Query String: Pass DoctorId to the details page.
o Hidden Field: Submit DoctorId with the appointment form.
4. Caching
o Implement Response Caching for the doctor details page.
o Cache the doctor list using IMemoryCache with a 3-minute expiration.
