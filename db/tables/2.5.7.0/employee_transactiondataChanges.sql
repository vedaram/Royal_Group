alter table employee_transactiondata drop column  WorkHourPerday 
alter table employee_transactiondata drop column WorkHourPerWeek 
alter table employee_transactiondata drop column WorkHourPerMonth 

alter table employee_transactiondata add   WorkHourPerday int 
alter table employee_transactiondata add  WorkHourPerWeek  int 
alter table employee_transactiondata add  WorkHourPerMonth int 

