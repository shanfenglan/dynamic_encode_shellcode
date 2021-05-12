import argparse
import struct
import sys

tem = []
final_tem = []
key = 0x21



def parse_args():
    # parse the arguments
    parser = argparse.ArgumentParser(epilog='\tExample: \r\npython3 ' + sys.argv[0] + " -s payload.bin -d target.bin")
    parser._optionals.title = "OPTIONS"
    parser.usage = "python3 shellcode.py -s 源bin文件路径 -t 混淆后生成的bin文件路径"
    parser.add_argument('-t', '--target', help="生成的目标文件",default="target.bin")
    parser.add_argument('-s', '--source', help='原bin文件路径', default="payload.bin")
    return parser.parse_args()

def xor():
    for i in range(len(tem)):
        x = i % 10 + key +1
        final_tem.append(tem[i] ^ x)


def readbin(str):

    with open(str, 'rb') as file:
        for i in file.read():
            tem.append(i)
        print(len(tem))



def writebin(str):
    xor()
    with open(str, 'wb') as file:
        for i in final_tem:
            x = struct.pack("B", i)
            file.write(x)


if __name__ == '__main__':
    value = parse_args()
    source = value.source
    target = value.target

    try:
        readbin(source)
        writebin(target)
    except:
        print("usage: python3 shellcode.py -s 源bin文件路径 -t 混淆后生成的bin文件路径")
