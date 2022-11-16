import React from "react";
import { Row, Form, Col, Input, Button, message, Image } from "antd";
import axios from "axios";
import utils from "../../common/Utils";
import "./Login.css";
import "../../store";
import store from "../../store";
import login_logo from "../../images/logo-color.png";
import { instanceOf } from "prop-types";
import { withCookies, Cookies } from "react-cookie";
import { Link } from "react-router-dom";
import {USER_LOGIN_ROUTE, USER_INFO_ROUTE} from "../../common/urls";

// DON'T USE THIS FILE, USE 'login.js' INSTESAD

class Login extends React.Component {
    static propTypes = {
        cookies: instanceOf(Cookies).isRequired,
    };

    constructor(props) {
        super(props);

        this.onFinish = this.onFinish.bind(this);
        this.checkLogin = this.checkLogin.bind(this);
    }

    componentDidMount() {
        window.document.title = "Login - JLLSS";

        this.checkLogin();
    }

    setLoading(bLoading) {
        let action = {
            type: "setLoading",
            value: bLoading,
        };

        store.dispatch(action);
    }

    onFinish(values) {
        this.setLoading(true);

        let self = this;
        axios
            .post(USER_LOGIN_ROUTE, values)
            .then(function (res) {
                console.log(res);   
                if (200 === res.data.code) {
                    const { cookies } = self.props;
                    cookies.set("token", res.data.data.token, { path: "/" });

                    self.props.history.push("/main");
                } else {
                    message.error(res.data.message);
                }
                self.setLoading(false);
            })
            .catch(function (err) {
                message.error(err.message);
                console.log(err);
                self.setLoading(false);
            });
    }


    checkLogin() {
        let self = this;
        const { cookies } = self.props;

        if (!cookies.get("token")) {
            return;
        }
        // Get user information API
        axios({
            method: "GET",
            url: USER_INFO_ROUTE,
            headers: { token: cookies.get("token") },
        })
            .then(function (res) {
                if (0 === res.data.code) {
                    self.props.history.push("/main");
                }
            })
            .catch(function (err) {
                message.error(err.message);
            });
    }

    render() {
        return (
            <Row justify="center" align="middle" className="container">
                <Col span={10}>
                    <Row className="logo">
                        <Image preview={false} src={login_logo} />
                    </Row>
                    {/* User Enter username & password */}

                    <Form name="basic" onFinish={this.onFinish}>
                        <Form.Item
                            name="username"
                            rules={[
                                {
                                    required: true,
                                    message: "Please input the username",
                                },
                            ]}
                        >
                            <Input placeholder="Username" size="large" />
                        </Form.Item>

                        <Form.Item
                            name="password"
                            rules={[
                                {
                                    required: true,
                                    message: "Please input the password",
                                },
                            ]}
                        >
                            <Input.Password placeholder="Password" size="large" />
                        </Form.Item>

                        <Form.Item>
                            {/* Click to login */}
                            <Button type="primary" htmlType="submit" block>
                                Login
                            </Button>
                        </Form.Item>
                        {/* Click to SignUp */}
                        <Button color="primary" onClick={""} block>
                            <Link to={"/SignUp"} target="_blank">
                                SignUp
                            </Link>
                        </Button>
                    </Form>
                </Col>
            </Row>
        );
    }
}

export default withCookies(Login);