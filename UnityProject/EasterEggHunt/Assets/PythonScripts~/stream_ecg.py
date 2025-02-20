import getopt
import sys
import time
from random import random as rand

from pylsl import StreamInfo, StreamOutlet, local_clock

import pyxdf


def main(argv):
    
    data, header = pyxdf.load_xdf("physio_sample.xdf")
    xdf_stream = None
    for i, stream in enumerate(data):
        if stream['info']['name'][0] ==  "ECG638C":
            xdf_stream = stream
            break

    # ECG Stream
    srate = 512
    name = "ECG"
    type = "ECG"
    n_channels = 1
    info = StreamInfo(name, type, n_channels, srate, "float32", "PLUME_ECG")
    outlet = StreamOutlet(info)

    sent_samples = 0
    
    while True:
        timestamp = local_clock()
        sample = xdf_stream['time_series'][sent_samples % len(stream['time_series'])]
        outlet.push_sample(sample, timestamp)
        sent_samples += 1
        time.sleep(1/srate)
        


if __name__ == "__main__":
    main(sys.argv[1:])