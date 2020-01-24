# WIFImonitor
IP monitor by using ping or http access

is configured by file WIFIMonitor.cfg



sample of  file WifiMonitor.cfg:
#comment 
# maximal count of devices is 35
#file WIFIMonitor.cfg
#refresh is in miliseconds
$refresh=60000
#objects monitored by ping
WButbnC;192.168.200.135
WbutbnE;192.168.200.118
# $HW device monitored by trying to open http page
$HWSkleniky;192.168.200.123:8001

