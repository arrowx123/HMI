#!/usr/bin/env python3
from OSC import OSCServer
import sys
from time import sleep

# ip address: receiving the coordiantes from accelerometer
server = OSCServer( ("192.168.27.1", 7110) )
server.timeout = 0
run = True

# this method of reporting timeouts only works by convention
# that before calling handle_request() field .timed_out is 
# set to False
def handle_timeout(self):
    self.timed_out = True

import types
server.handle_timeout = types.MethodType(handle_timeout, server)

def user_callback(path, tags, args, source):
    user = ''.join(path.split(","))
    # tags will contain 'fff'
    # args is a OSCMessage with data
    # source is where the message came from. args array contains coordinates x,y,z
    print ("received", args[0]/1000.0, args[1]/1000.0, args[2]/1000.0) 

def quit_callback(path, tags, args, source):
    global run
    run = False

server.addMsgHandler( "/osc", user_callback )
server.addMsgHandler( "/user/2", user_callback )
server.addMsgHandler( "/user/3", user_callback )
server.addMsgHandler( "/user/4", user_callback )
server.addMsgHandler( "/quit", quit_callback )

def each_frame():
    # clear timed_out flag
    server.timed_out = False
    # handle all pending requests then return
    while not server.timed_out:
        server.handle_request()

while run:
    # refresh frame every 500ms
    sleep(0.5)
    # call user script
    each_frame()

server.close()
