void setup()
{
  String tmp = "/setOSCAddress/172.16.202.201/setOSCAddress";
  String currentCommand = "setOSCAddress";
  
  int firstPos = tmp.indexOf("setOSCAddress");
  int secondPos = tmp.indexOf("setOSCAddress", firstPos + 1);

  String IP = tmp.substring(firstPos + currentCommand.length() + 1, secondPos - 1);
  println("IP is: " + IP);
}