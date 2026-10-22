// Ejecutar desde mongosh contra la base facturaflow: load('scripts/seed.js')
// Los IDs se generan automáticamente. Script idempotente por número de factura.
const dbx = db.getSiblingDB('facturaflow');
const invoices = [
 {clienteId:'CLI-001',clienteNombre:'Comercial Andina',emailCliente:'cliente1@example.com',numero:'FAC-2026-001',valor:450000,fechaVencimiento:new Date('2026-09-15T00:00:00Z'),estado:'primerrecordatorio',ultimoRecordatorio:null,historialEnvios:[]},
 {clienteId:'CLI-001',clienteNombre:'Comercial Andina',emailCliente:'cliente1@example.com',numero:'FAC-2026-002',valor:285000,fechaVencimiento:new Date('2026-09-20T00:00:00Z'),estado:'segundorecordatorio',ultimoRecordatorio:null,historialEnvios:[]},
 {clienteId:'CLI-002',clienteNombre:'Soluciones del Norte',emailCliente:'cliente2@example.com',numero:'FAC-2026-003',valor:690000,fechaVencimiento:new Date('2026-09-22T00:00:00Z'),estado:'primerrecordatorio',ultimoRecordatorio:null,historialEnvios:[]},
 {clienteId:'CLI-002',clienteNombre:'Soluciones del Norte',emailCliente:'cliente2@example.com',numero:'FAC-2026-004',valor:175000,fechaVencimiento:new Date('2026-09-28T00:00:00Z'),estado:'desactivado',ultimoRecordatorio:null,historialEnvios:[]},
 {clienteId:'CLI-003',clienteNombre:'Distribuciones Horizonte',emailCliente:'cliente3@example.com',numero:'FAC-2026-005',valor:820000,fechaVencimiento:new Date('2026-10-01T00:00:00Z'),estado:'segundorecordatorio',ultimoRecordatorio:null,historialEnvios:[]},
 {clienteId:'CLI-003',clienteNombre:'Distribuciones Horizonte',emailCliente:'cliente3@example.com',numero:'FAC-2026-006',valor:320000,fechaVencimiento:new Date('2026-10-05T00:00:00Z'),estado:'primerrecordatorio',ultimoRecordatorio:null,historialEnvios:[]}
];
for (const invoice of invoices) dbx.facturas.updateOne({numero:invoice.numero},{$setOnInsert:invoice}, {upsert:true});
print(`Seed completado. Facturas registradas: ${dbx.facturas.countDocuments()}`);
