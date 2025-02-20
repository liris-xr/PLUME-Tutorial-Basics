import getopt
import sys
import time
from random import random as rand

from pylsl import StreamInfo, StreamOutlet, local_clock

import pyxdf


def main(argv):
    
    data, header = pyxdf.load_xdf("physio_sample.xdf")
    stream_eda_left = None
    stream_eda_right = None
    for i, stream in enumerate(data):
        if stream['info']['name'][0] ==  "EDA74FB":
            stream_eda_left = stream
        if stream['info']['name'][0] ==  "EDA850D":
            stream_eda_right = stream

    # EDA Left Stream
    srate = 8
    name = "EDA_Left"
    type = "EDA"
    n_channels = 1
    info = StreamInfo(name, type, n_channels, srate, "float32", "PLUME_EDA_LEFT")
    outlet_eda_left = StreamOutlet(info)

    # EDA Right Stream
    srate = 8
    name = "EDA_Right"
    type = "EDA"
    n_channels = 1
    info = StreamInfo(name, type, n_channels, srate, "float32", "PLUME_EDA_Right")
    outlet_eda_right = StreamOutlet(info)

    sent_samples = 0
    
    while True:
        timestamp = local_clock()

        sample_eda_left = stream_eda_left['time_series'][sent_samples % len(stream_eda_left['time_series'])]
        outlet_eda_left.push_sample(sample_eda_left, timestamp)

        sample_eda_right = stream_eda_right['time_series'][sent_samples % len(stream_eda_right['time_series'])]
        outlet_eda_right.push_sample(sample_eda_right, timestamp)

        sent_samples += 1
        time.sleep(1/srate)
        


if __name__ == "__main__":
    main(sys.argv[1:])