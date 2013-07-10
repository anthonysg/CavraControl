#!/usr/bin/env python
# encoding: utf-8
# Copyright (c) 2012 SjB <steve@nca.uwo.ca>. All Rights Reserved.

import os
from waflib import Options

APPNAME = 'CavraControl'
VERSION = '0.0.1'

top = '.'
out = 'build'
def options(ctx):
    ctx.load('cs_extra etoform', tooldir='waftools')

    ctx.add_option('--debug', '-d',
                dest='debug',
                action='store_true',
                default=False,
                help='Enable debug build')
    ctx.add_option('--default-assembly-dir',
                dest='default_assembly_dir',
                type='string',
                default=False,
                help='Location of local assembly repository')

def configure(ctx):
    ctx.load('cs_extra etoform', tooldir='waftools')

    ctx.env.APPNAME = APPNAME
    ctx.define_cond('DEBUG', Options.options.debug)

    ctx.env.default_assembly_dir = Options.options.default_assembly_dir
    if not ctx.env.default_assembly_dir:
        ctx.env.default_assembly_dir = os.environ.get('DEFAULT_ASSEMBLY_DIR', './libs')

    ctx.check_etoform(path_list = generate_path_list(ctx.env.default_assembly_dir, 'Eto'))

    libusb_libdir = '{0}/SjB.Hid'.format(ctx.env.default_assembly_dir)
    ctx.check_assembly('SjB.Hid', path_list = libusb_libdir);

    ctx.env.default_app_install_path = '${PREFIX}/lib/%s' % APPNAME

def generate_path_list(path, module):
        return '{0} {0}/{1}'.format(path, module)

def build(ctx):
    ctx.recurse([
        'NCA.CavraDriver',
 #       'Cavra Control'
        'Test'
    ])
